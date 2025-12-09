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
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<IdentityUser> userManager,
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _configuration = configuration;
    }

    /// <summary>
    /// Registra un nuevo empleado y crea su cuenta de usuario
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if email already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "El email ya está registrado" });
        }

        // Validate department exists
        var department = await _departmentRepository.GetByIdAsync(request.DepartmentId);
        if (department == null)
        {
            return BadRequest(new { message = "El departamento especificado no existe" });
        }

        // Parse education level
        if (!Enum.TryParse<EducationLevel>(request.EducationLevel, true, out var educationLevel))
        {
            return BadRequest(new { message = "Nivel educativo no válido. Valores permitidos: HighSchool, Technical, Bachelor, Master, Doctorate" });
        }

        // Create Identity user
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

        // Assign Employee role
        await _userManager.AddToRoleAsync(user, "Employee");

        // Create employee record
        var employee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = DateTime.SpecifyKind(request.DateOfBirth, DateTimeKind.Utc),
            Address = request.Address,
            Phone = request.Phone,
            Email = request.Email,
            Position = request.Position,
            Salary = request.Salary,
            HireDate = DateTime.SpecifyKind(request.HireDate, DateTimeKind.Utc),
            Status = EmployeeStatus.Active,
            EducationLevel = educationLevel,
            ProfessionalProfile = request.ProfessionalProfile,
            DepartmentId = request.DepartmentId,
            UserId = user.Id
        };

        await _employeeRepository.CreateAsync(employee);

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

    /// <summary>
    /// Inicia sesión y devuelve un token JWT
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

        if (employee.Status != EmployeeStatus.Active)
        {
            return Unauthorized(new { message = "Su cuenta de empleado no está activa" });
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
