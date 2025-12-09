using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Enums;
using TalentoPlus.Core.Interfaces;

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

    public async Task<IActionResult> Index()
    {
        var employees = await _employeeRepository.GetAllAsync();
        return View(employees);
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
    public async Task<IActionResult> Create(Employee employee)
    {
        if (ModelState.IsValid)
        {
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

        if (ModelState.IsValid)
        {
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
        ViewBag.Departments = new SelectList(departments, "Id", "Name");

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
