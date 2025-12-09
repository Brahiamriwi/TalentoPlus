using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Web.Controllers;

[Authorize(Roles = "Admin")]
public class ExcelController : Controller
{
    private readonly IExcelService _excelService;

    public ExcelController(IExcelService excelService)
    {
        _excelService = excelService;
    }

    public IActionResult Import()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Por favor, selecciona un archivo Excel.";
            return View();
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".xlsx" && extension != ".xls")
        {
            TempData["Error"] = "El archivo debe ser un Excel (.xlsx o .xls).";
            return View();
        }

        try
        {
            using var stream = file.OpenReadStream();
            var importedCount = await _excelService.ImportEmployeesAsync(stream);

            if (importedCount > 0)
            {
                TempData["Success"] = $"Se importaron {importedCount} empleado(s) exitosamente.";
            }
            else
            {
                TempData["Warning"] = "No se importaron empleados. Verifica que el archivo tenga datos válidos.";
            }

            return RedirectToAction("Index", "Employee");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al importar: {ex.Message}";
            return View();
        }
    }
}
