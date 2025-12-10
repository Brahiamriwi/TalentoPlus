using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Enums;
using TalentoPlus.Core.Interfaces;
using TalentoPlus.Web.Models;

namespace TalentoPlus.Web.Controllers;

[Authorize(Roles = "Admin")]
public class EmployeeController : Controller
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IPdfService _pdfService;

    public EmployeeController(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IPdfService pdfService)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _pdfService = pdfService;
    }

    public async Task<IActionResult> Index(string? searchTerm, int? departmentId, EmployeeStatus? status, int page = 1, int pageSize = 10)
    {
        var allEmployees = await _employeeRepository.GetAllAsync();
        var query = allEmployees.AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(e => 
                e.FirstName.ToLower().Contains(term) ||
                e.LastName.ToLower().Contains(term) ||
                e.Email.ToLower().Contains(term) ||
                e.Position.ToLower().Contains(term) ||
                (e.Department != null && e.Department.Name.ToLower().Contains(term)));
        }

        if (departmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == departmentId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(e => e.Status == status.Value);
        }

        // Contar total antes de paginar
        var totalItems = query.Count();

        // Aplicar paginación
        var employees = query
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var departments = await _departmentRepository.GetAllAsync();

        var viewModel = new EmployeeIndexViewModel
        {
            Employees = employees,
            SearchTerm = searchTerm,
            DepartmentId = departmentId,
            Status = status,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            Departments = departments
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
            return NotFound();

        return View(employee);
    }

    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee employee, string? otherDepartmentName)
    {
        // Handle "Other" department option FIRST before validation
        if (employee.DepartmentId == -1)
        {
            if (!string.IsNullOrWhiteSpace(otherDepartmentName))
            {
                var newDepartment = new Department { Name = otherDepartmentName.Trim() };
                await _departmentRepository.CreateAsync(newDepartment);
                employee.DepartmentId = newDepartment.Id;
                // Remove the validation error for DepartmentId since we just set it
                ModelState.Remove("DepartmentId");
            }
            else
            {
                ModelState.AddModelError("DepartmentId", "Debe especificar el nombre del nuevo departamento.");
            }
        }

        // Server-side validation for date of birth (must be 18+)
        var age = DateTime.Today.Year - employee.DateOfBirth.Year;
        if (employee.DateOfBirth > DateTime.Today.AddYears(-age)) age--;
        
        if (age < 18)
        {
            ModelState.AddModelError("DateOfBirth", "El empleado debe tener al menos 18 años de edad.");
        }

        // Server-side validation for hire date (cannot be future)
        if (employee.HireDate.Date > DateTime.Today)
        {
            ModelState.AddModelError("HireDate", "La fecha de ingreso no puede ser una fecha futura.");
        }

        // Remove Department navigation property validation error if exists
        ModelState.Remove("Department");

        if (ModelState.IsValid)
        {
            // Check for duplicate email
            var allEmployees = await _employeeRepository.GetAllAsync();
            var existingEmail = allEmployees.FirstOrDefault(e => 
                e.Email.ToLower() == employee.Email.ToLower());
            
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Ya existe un empleado con este correo electrónico.");
                await LoadDropdowns();
                return View(employee);
            }

            // Check for duplicate document
            var existingDocument = allEmployees.FirstOrDefault(e => 
                e.Document == employee.Document);
            
            if (existingDocument != null)
            {
                ModelState.AddModelError("Document", "Ya existe un empleado con este documento de identidad.");
                await LoadDropdowns();
                return View(employee);
            }

            // Ensure dates are UTC for PostgreSQL
            employee.DateOfBirth = DateTime.SpecifyKind(employee.DateOfBirth, DateTimeKind.Utc);
            employee.HireDate = DateTime.SpecifyKind(employee.HireDate, DateTimeKind.Utc);
            
            await _employeeRepository.CreateAsync(employee);
            TempData["Success"] = "Empleado creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        await LoadDropdowns();
        return View(employee);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
            return NotFound();

        await LoadDropdowns();
        return View(employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Employee employee)
    {
        if (id != employee.Id)
            return NotFound();

        // Remove Department navigation property validation error if exists
        ModelState.Remove("Department");

        // Server-side validation for date of birth (must be 18+)
        var age = DateTime.Today.Year - employee.DateOfBirth.Year;
        if (employee.DateOfBirth > DateTime.Today.AddYears(-age)) age--;
        
        if (age < 18)
        {
            ModelState.AddModelError("DateOfBirth", "El empleado debe tener al menos 18 años de edad.");
        }

        // Server-side validation for hire date (cannot be future)
        if (employee.HireDate.Date > DateTime.Today)
        {
            ModelState.AddModelError("HireDate", "La fecha de ingreso no puede ser una fecha futura.");
        }

        if (ModelState.IsValid)
        {
            // Check for duplicate email (excluding current employee)
            var allEmployees = await _employeeRepository.GetAllAsync();
            var existingEmail = allEmployees.FirstOrDefault(e => 
                e.Email.ToLower() == employee.Email.ToLower() && e.Id != employee.Id);
            
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Ya existe otro empleado con este correo electrónico.");
                await LoadDropdowns();
                return View(employee);
            }

            // Check for duplicate document (excluding current employee)
            var existingDocument = allEmployees.FirstOrDefault(e => 
                e.Document == employee.Document && e.Id != employee.Id);
            
            if (existingDocument != null)
            {
                ModelState.AddModelError("Document", "Ya existe otro empleado con este documento de identidad.");
                await LoadDropdowns();
                return View(employee);
            }

            // Ensure dates are UTC for PostgreSQL
            employee.DateOfBirth = DateTime.SpecifyKind(employee.DateOfBirth, DateTimeKind.Utc);
            employee.HireDate = DateTime.SpecifyKind(employee.HireDate, DateTimeKind.Utc);
            
            await _employeeRepository.UpdateAsync(employee);
            TempData["Success"] = "Empleado actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        await LoadDropdowns();
        return View(employee);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
            return NotFound();

        return View(employee);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _employeeRepository.DeleteAsync(id);
        TempData["Success"] = "Empleado eliminado exitosamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DownloadResume(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
            return NotFound();

        var pdfBytes = await _pdfService.GenerateResumeAsync(employee);
        var fileName = $"CV_{employee.FirstName}_{employee.LastName}.pdf";

        return File(pdfBytes, "application/pdf", fileName);
    }

    private async Task LoadDropdowns()
    {
        var departments = await _departmentRepository.GetAllAsync();
        var departmentList = departments.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name }).ToList();
        departmentList.Add(new SelectListItem { Value = "-1", Text = "Otro (especificar)" });
        ViewBag.Departments = departmentList;

        ViewBag.StatusList = new SelectList(
            Enum.GetValues<EmployeeStatus>().Select(e => new { Value = (int)e, Text = GetStatusDisplay(e) }),
            "Value", "Text");

        ViewBag.EducationLevelList = new SelectList(
            Enum.GetValues<EducationLevel>().Select(e => new { Value = (int)e, Text = GetEducationLevelDisplay(e) }),
            "Value", "Text");
    }

    private static string GetStatusDisplay(EmployeeStatus status)
    {
        return status switch
        {
            EmployeeStatus.Active => "Activo",
            EmployeeStatus.Inactive => "Inactivo",
            EmployeeStatus.OnVacation => "En Vacaciones",
            _ => status.ToString()
        };
    }

    private static string GetEducationLevelDisplay(EducationLevel level)
    {
        return level switch
        {
            EducationLevel.HighSchool => "Bachiller",
            EducationLevel.Technical => "Técnico",
            EducationLevel.Technologist => "Tecnólogo",
            EducationLevel.Bachelor => "Profesional",
            EducationLevel.Specialization => "Especialización",
            EducationLevel.Master => "Maestría",
            EducationLevel.Doctorate => "Doctorado",
            _ => level.ToString()
        };
    }
}
