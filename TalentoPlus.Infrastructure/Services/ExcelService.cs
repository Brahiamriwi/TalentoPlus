using OfficeOpenXml;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Enums;
using TalentoPlus.Core.Interfaces;

namespace TalentoPlus.Infrastructure.Services;

public class ExcelService : IExcelService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public ExcelService(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<int> ImportEmployeesAsync(Stream fileStream)
    {
        var importedCount = 0;

        using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        
        if (worksheet == null)
            return 0;

        var rowCount = worksheet.Dimension?.Rows ?? 0;
        
        // Start from row 2 (skip header)
        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                var departmentName = worksheet.Cells[row, 13].Text?.Trim();
                if (string.IsNullOrEmpty(departmentName))
                    continue;

                // Find or create department
                var department = await _departmentRepository.GetByNameAsync(departmentName);
                if (department == null)
                {
                    department = await _departmentRepository.CreateAsync(new Department { Name = departmentName });
                }

                var email = worksheet.Cells[row, 6].Text?.Trim();
                if (string.IsNullOrEmpty(email))
                    continue;

                // Check if employee already exists
                var existingEmployee = await _employeeRepository.GetByEmailAsync(email);
                if (existingEmployee != null)
                    continue;

                var employee = new Employee
                {
                    FirstName = worksheet.Cells[row, 1].Text?.Trim() ?? string.Empty,
                    LastName = worksheet.Cells[row, 2].Text?.Trim() ?? string.Empty,
                    DateOfBirth = ParseDate(worksheet.Cells[row, 3].Text),
                    Address = worksheet.Cells[row, 4].Text?.Trim() ?? string.Empty,
                    Phone = worksheet.Cells[row, 5].Text?.Trim() ?? string.Empty,
                    Email = email,
                    Position = worksheet.Cells[row, 7].Text?.Trim() ?? string.Empty,
                    Salary = ParseDecimal(worksheet.Cells[row, 8].Text),
                    HireDate = ParseDate(worksheet.Cells[row, 9].Text),
                    Status = ParseEnum<EmployeeStatus>(worksheet.Cells[row, 10].Text),
                    EducationLevel = ParseEnum<EducationLevel>(worksheet.Cells[row, 11].Text),
                    ProfessionalProfile = worksheet.Cells[row, 12].Text?.Trim() ?? string.Empty,
                    DepartmentId = department.Id
                };

                await _employeeRepository.CreateAsync(employee);
                importedCount++;
            }
            catch
            {
                // Skip rows with errors and continue with next row
                continue;
            }
        }

        return importedCount;
    }

    private static DateTime ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return DateTime.MinValue;

        if (DateTime.TryParse(value, out var date))
            return date;

        // Try parsing Excel numeric date
        if (double.TryParse(value, out var numericDate))
            return DateTime.FromOADate(numericDate);

        return DateTime.MinValue;
    }

    private static decimal ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0;

        // Remove currency symbols and thousands separators
        var cleanValue = value.Replace("$", "").Replace(",", "").Trim();
        
        if (decimal.TryParse(cleanValue, out var result))
            return result;

        return 0;
    }

    private static T ParseEnum<T>(string? value) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return default;

        if (Enum.TryParse<T>(value.Trim(), ignoreCase: true, out var result))
            return result;

        return default;
    }
}
