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
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<IdentityUser> userManager,
        IEmployeeRepository employeeRepository,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _employeeRepository = employeeRepository;
        _emailService = emailService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var employees = await _employeeRepository.GetAllAsync();
        var employee = employees.FirstOrDefault(e => e.Document == request.Document);

        if (employee == null)
            return NotFound(new { message = "No se encontró ningún empleado con ese documento. Contacta a RRHH." });

        if (!string.IsNullOrEmpty(employee.UserId))
            return BadRequest(new { message = "Ya tienes una cuenta. Revisa tu correo para las credenciales o contacta a RRHH." });

        var password = GenerateSecurePassword();
        var existingUser = await _userManager.FindByEmailAsync(employee.Email);
        IdentityUser user;

        if (existingUser != null)
        {
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
            var resetResult = await _userManager.ResetPasswordAsync(existingUser, resetToken, password);
            if (!resetResult.Succeeded)
                return BadRequest(new { message = "Error al configurar credenciales", errors = resetResult.Errors.Select(e => e.Description) });
            
            user = existingUser;
            
            if (!await _userManager.IsInRoleAsync(user, "Employee"))
                await _userManager.AddToRoleAsync(user, "Employee");
        }
        else
        {
            user = new IdentityUser
            {
                UserName = employee.Email,
                Email = employee.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return BadRequest(new { message = "Error al crear la cuenta", errors = result.Errors.Select(e => e.Description) });

            await _userManager.AddToRoleAsync(user, "Employee");
        }

        employee.UserId = user.Id;
        await _employeeRepository.UpdateAsync(employee);

        var emailSent = await _emailService.SendCredentialsEmailAsync(employee.Email, $"{employee.FirstName} {employee.LastName}", password);

        if (!emailSent)
        {
            employee.UserId = null;
            await _employeeRepository.UpdateAsync(employee);
            return StatusCode(500, new { message = "No se pudo enviar el correo con las credenciales. Intenta de nuevo." });
        }

        return Ok(new { message = $"¡Cuenta creada! Se enviaron las credenciales a {MaskEmail(employee.Email)}" });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized(new { message = "Credenciales inválidas" });

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized(new { message = "Credenciales inválidas" });

        if (!await _userManager.IsInRoleAsync(user, "Employee"))
            return Unauthorized(new { message = "Esta API es solo para empleados." });

        var employees = await _employeeRepository.GetAllAsync();
        var employee = employees.FirstOrDefault(e => e.UserId == user.Id);
        
        if (employee == null)
            return Unauthorized(new { message = "No se encontró el registro de empleado asociado" });

        if (employee.Status == EmployeeStatus.Inactive)
            return Unauthorized(new { message = "Tu cuenta está pendiente de activación por RRHH." });

        return Ok(new AuthResponseDto
        {
            Token = GenerateJwtToken(user, employee),
            Expiration = DateTime.UtcNow.AddHours(24),
            Email = user.Email!,
            FullName = $"{employee.FirstName} {employee.LastName}"
        });
    }

    private static string GenerateSecurePassword()
    {
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%&*";
        
        var random = new Random();
        var password = new char[12];
        
        password[0] = upper[random.Next(upper.Length)];
        password[1] = lower[random.Next(lower.Length)];
        password[2] = digits[random.Next(digits.Length)];
        password[3] = special[random.Next(special.Length)];
        
        var all = upper + lower + digits + special;
        for (int i = 4; i < 12; i++)
            password[i] = all[random.Next(all.Length)];
        
        return new string(password.OrderBy(_ => random.Next()).ToArray());
    }

    private static string MaskEmail(string email)
    {
        var parts = email.Split('@');
        if (parts[0].Length <= 3)
            return $"{parts[0][0]}***@{parts[1]}";
        return $"{parts[0][..3]}***@{parts[1]}";
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

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
