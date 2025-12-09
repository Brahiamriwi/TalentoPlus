using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Core.Enums;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Web.Controllers;

[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IOpenAIService _openAIService;

    public DashboardController(IEmployeeRepository employeeRepository, IOpenAIService openAIService)
    {
        _employeeRepository = employeeRepository;
        _openAIService = openAIService;
    }

    public async Task<IActionResult> Index()
    {
        var employees = (await _employeeRepository.GetAllAsync()).ToList();

        ViewBag.TotalEmployees = employees.Count;
        ViewBag.ActiveEmployees = employees.Count(e => e.Status == EmployeeStatus.Active);
        ViewBag.OnVacationEmployees = employees.Count(e => e.Status == EmployeeStatus.OnVacation);
        ViewBag.InactiveEmployees = employees.Count(e => e.Status == EmployeeStatus.Inactive);

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AskAI([FromBody] AskAIRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Query))
        {
            return Json(new { success = false, response = "Por favor, ingresa una consulta." });
        }

        try
        {
            var employees = (await _employeeRepository.GetAllAsync()).ToList();
            var response = await _openAIService.ProcessNaturalLanguageQueryAsync(request.Query, employees);
            return Json(new { success = true, response });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, response = $"Error al procesar la consulta: {ex.Message}" });
        }
    }
}

public class AskAIRequest
{
    public string Query { get; set; } = string.Empty;
}
