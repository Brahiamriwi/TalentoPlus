using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TalentoPlus.Core.DTOs;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Enums;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<IdentityUser> userManager,
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _emailService = emailService;
        _configuration = configuration;
    }

    /// <summary>
    /// Autoregistro de empleado. Si el empleado ya existe (importado desde Excel),
    /// solo crea las credenciales de acceso. Si no existe, crea empleado nuevo.
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if Identity user already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Ya tienes una cuenta creada. Por favor, inicia sesión." });
        }

        var employees = await _employeeRepository.GetAllAsync();
        var existingEmployee = employees.FirstOrDefault(e => 
            e.Document == request.Document || 
            e.Email.ToLower() == request.Email.ToLower());

        Employee employee;
        string mensaje;

        if (existingEmployee != null)
        {
        
            if (!string.IsNullOrEmpty(existingEmployee.UserId))
            {
                return BadRequest(new { message = "Este empleado ya tiene una cuenta de acceso creada. Por favor, inicia sesión." });
            }

            employee = existingEmployee;
            mensaje = "¡Bienvenido! Hemos vinculado tu cuenta con tu registro de empleado existente.";
        }
        else
        {
    
            var department = await _departmentRepository.GetByIdAsync(request.DepartmentId);
            if (department == null)
            {
                return BadRequest(new { message = "El departamento especificado no existe" });
            }

            employee = new Employee
            {
                Document = request.Document,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                DateOfBirth = DateTime.SpecifyKind(DateTime.Today.AddYears(-25), DateTimeKind.Utc),
                Address = "Pendiente de actualización",
                Position = "Pendiente de asignación",
                Salary = 0,
                HireDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc),
                Status = EmployeeStatus.Inactive,
                EducationLevel = EducationLevel.HighSchool,
                ProfessionalProfile = "Pendiente de actualización",
                DepartmentId = request.DepartmentId
            };

            await _employeeRepository.CreateAsync(employee);
            mensaje = "Registro exitoso. Tu cuenta está pendiente de activación por RRHH.";
        }

        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return BadRequest(new { message = "Error al crear el usuario", errors = createResult.Errors.Select(e => e.Description) });
        }

        await _userManager.AddToRoleAsync(user, "Employee");

    
        employee.UserId = user.Id;
        
        if (existingEmployee != null)
        {
    
            if (!string.IsNullOrEmpty(request.Phone) && request.Phone != employee.Phone)
            {
                employee.Phone = request.Phone;
            }
        }
        
        await _employeeRepository.UpdateAsync(employee);

        var fullName = $"{employee.FirstName} {employee.LastName}";
        var emailSent = await _emailService.SendWelcomeEmailAsync(request.Email, fullName);

    
        var token = GenerateJwtToken(user, employee);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(24),
            Email = user.Email!,
            FullName = fullName,
            Message = emailSent 
                ? $"{mensaje} Se ha enviado un correo de bienvenida."
                : $"{mensaje} No se pudo enviar el correo de bienvenida."
        });
    }

    /// <summary>
    /// Login de empleado. Devuelve token JWT si las credenciales son válidas.
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Credenciales inválidas" });
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isValidPassword)
        {
            return Unauthorized(new { message = "Credenciales inválidas" });
        }

        // Check if user is an employee (not admin)
        var isEmployee = await _userManager.IsInRoleAsync(user, "Employee");
        if (!isEmployee)
        {
            return Unauthorized(new { message = "Esta API es solo para empleados. Los administradores deben usar el portal web." });
        }

        // Get employee record
        var employees = await _employeeRepository.GetAllAsync();
        var employee = employees.FirstOrDefault(e => e.UserId == user.Id);
        
        if (employee == null)
        {
            return Unauthorized(new { message = "No se encontró el registro de empleado asociado" });
        }

        if (employee.Status == EmployeeStatus.Inactive)
        {
            return Unauthorized(new { message = "Tu cuenta está pendiente de activación por el departamento de RRHH." });
        }

        // Generate JWT token
        var token = GenerateJwtToken(user, employee);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(24),
            Email = user.Email!,
            FullName = $"{employee.FirstName} {employee.LastName}"
        });
    }

    private string GenerateJwtToken(IdentityUser user, Employee employee)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "TalentoPlusAPI";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "TalentoPlusUsers";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, $"{employee.FirstName} {employee.LastName}"),
            new("EmployeeId", employee.Id.ToString()),
            new("Document", employee.Document),
            new(ClaimTypes.Role, "Employee")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(24);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
