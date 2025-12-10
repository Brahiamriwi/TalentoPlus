using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Core.DTOs;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Employee")]
public class EmpleadoController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPdfService _pdfService;

    public EmpleadoController(
        IEmployeeRepository employeeRepository,
        IPdfService pdfService)
    {
        _employeeRepository = employeeRepository;
        _pdfService = pdfService;
    }

    /// <summary>
    /// Obtiene la información del empleado autenticado
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<EmployeeResponseDto>> GetMyInfo()
    {
        var employeeId = GetEmployeeIdFromToken();
        if (employeeId == null)
        {
            return Unauthorized(new { message = "No se pudo identificar al empleado" });
        }

        var employee = await _employeeRepository.GetByIdAsync(employeeId.Value);
        if (employee == null)
        {
            return NotFound(new { message = "Empleado no encontrado" });
        }

        var response = new EmployeeResponseDto
        {
            Id = employee.Id,
            Document = employee.Document,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            DateOfBirth = employee.DateOfBirth,
            Address = employee.Address,
            Phone = employee.Phone,
            Email = employee.Email,
            Position = employee.Position,
            Salary = employee.Salary,
            HireDate = employee.HireDate,
            Status = employee.Status.ToString(),
            EducationLevel = employee.EducationLevel.ToString(),
            ProfessionalProfile = employee.ProfessionalProfile,
            DepartmentName = employee.Department?.Name ?? "Sin departamento"
        };

        return Ok(response);
    }

    /// <summary>
    /// Descarga la hoja de vida del empleado autenticado en formato PDF
    /// </summary>
    [HttpGet("me/pdf")]
    public async Task<IActionResult> DownloadMyResume()
    {
        var employeeId = GetEmployeeIdFromToken();
        if (employeeId == null)
        {
            return Unauthorized(new { message = "No se pudo identificar al empleado" });
        }

        var employee = await _employeeRepository.GetByIdAsync(employeeId.Value);
        if (employee == null)
        {
            return NotFound(new { message = "Empleado no encontrado" });
        }

        var pdfBytes = await _pdfService.GenerateResumeAsync(employee);
        var fileName = $"HojaDeVida_{employee.FirstName}_{employee.LastName}.pdf";

        return File(pdfBytes, "application/pdf", fileName);
    }

    /// <summary>
    /// Actualiza la información de contacto del empleado autenticado
    /// </summary>
    [HttpPut("me/contact")]
    public async Task<ActionResult<EmployeeResponseDto>> UpdateMyContact([FromBody] UpdateContactDto request)
    {
        var employeeId = GetEmployeeIdFromToken();
        if (employeeId == null)
        {
            return Unauthorized(new { message = "No se pudo identificar al empleado" });
        }

        var employee = await _employeeRepository.GetByIdAsync(employeeId.Value);
        if (employee == null)
        {
            return NotFound(new { message = "Empleado no encontrado" });
        }

        // Only allow updating contact information
        if (!string.IsNullOrWhiteSpace(request.Address))
            employee.Address = request.Address;
        
        if (!string.IsNullOrWhiteSpace(request.Phone))
            employee.Phone = request.Phone;

        await _employeeRepository.UpdateAsync(employee);

        var response = new EmployeeResponseDto
        {
            Id = employee.Id,
            Document = employee.Document,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            DateOfBirth = employee.DateOfBirth,
            Address = employee.Address,
            Phone = employee.Phone,
            Email = employee.Email,
            Position = employee.Position,
            Salary = employee.Salary,
            HireDate = employee.HireDate,
            Status = employee.Status.ToString(),
            EducationLevel = employee.EducationLevel.ToString(),
            ProfessionalProfile = employee.ProfessionalProfile,
            DepartmentName = employee.Department?.Name ?? "Sin departamento"
        };

        return Ok(response);
    }

    private int? GetEmployeeIdFromToken()
    {
        var employeeIdClaim = User.FindFirst("EmployeeId")?.Value;
        if (string.IsNullOrEmpty(employeeIdClaim))
            return null;

        if (int.TryParse(employeeIdClaim, out var employeeId))
            return employeeId;

        return null;
    }
}

public class UpdateContactDto
{
    public string? Address { get; set; }
    public string? Phone { get; set; }
}
