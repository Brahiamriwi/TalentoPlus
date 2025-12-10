using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Core.DTOs;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartamentosController : ControllerBase
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartamentosController(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentResponseDto>>> GetAll()
    {
        var departments = await _departmentRepository.GetAllAsync();
        
        var response = departments.Select(d => new DepartmentResponseDto
        {
            Id = d.Id,
            Name = d.Name,
            EmployeeCount = d.Employees?.Count ?? 0
        });

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DepartmentResponseDto>> GetById(int id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        
        if (department == null)
        {
            return NotFound(new { message = "Departamento no encontrado" });
        }

        var response = new DepartmentResponseDto
        {
            Id = department.Id,
            Name = department.Name,
            EmployeeCount = department.Employees?.Count ?? 0
        };

        return Ok(response);
    }
}
