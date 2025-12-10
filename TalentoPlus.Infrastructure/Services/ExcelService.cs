using OfficeOpenXml;
using System.Globalization;
using System.Text;
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
        
        // Step 1: Extract unique department names from Excel
        var departmentNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (int row = 2; row <= rowCount; row++)
        {
            var deptName = worksheet.Cells[row, 14].Text?.Trim(); // Departamento is column 14
            if (!string.IsNullOrEmpty(deptName))
                departmentNames.Add(deptName);
        }

        // Step 2: Ensure all departments exist in DB
        foreach (var deptName in departmentNames)
        {
            var existing = await _departmentRepository.GetByNameAsync(deptName);
            if (existing == null)
            {
                await _departmentRepository.CreateAsync(new Department { Name = deptName });
            }
        }

        // Step 3: Reload all departments into cache
        var allDepartments = await _departmentRepository.GetAllAsync();
        var deptCache = allDepartments.ToDictionary(d => d.Name, d => d.Id, StringComparer.OrdinalIgnoreCase);

        // Step 4: Import employees
        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                var rawEmail = worksheet.Cells[row, 7].Text?.Trim(); // Email is column 7
                if (string.IsNullOrEmpty(rawEmail))
                    continue;

                // Normalize email: remove accents and convert to lowercase
                var email = NormalizeEmail(rawEmail);
                if (string.IsNullOrEmpty(email))
                    continue;

                var existingEmployee = await _employeeRepository.GetByEmailAsync(email);
                if (existingEmployee != null)
                    continue;

                var departmentName = worksheet.Cells[row, 14].Text?.Trim(); // Departamento is column 14
                if (string.IsNullOrEmpty(departmentName) || !deptCache.TryGetValue(departmentName, out var deptId))
                    continue;

                var employee = new Employee
                {
                    Document = worksheet.Cells[row, 1].Text?.Trim() ?? string.Empty,       // Documento
                    FirstName = worksheet.Cells[row, 2].Text?.Trim() ?? string.Empty,      // Nombres
                    LastName = worksheet.Cells[row, 3].Text?.Trim() ?? string.Empty,       // Apellidos
                    DateOfBirth = ParseDate(worksheet.Cells[row, 4].Text),                 // FechaNacimiento
                    Address = worksheet.Cells[row, 5].Text?.Trim() ?? string.Empty,        // Direccion
                    Phone = worksheet.Cells[row, 6].Text?.Trim() ?? string.Empty,          // Telefono
                    Email = email,                                                          // Email (column 7) - normalized
                    Position = worksheet.Cells[row, 8].Text?.Trim() ?? string.Empty,       // Cargo
                    Salary = ParseDecimal(worksheet.Cells[row, 9].Text),                   // Salario
                    HireDate = ParseDate(worksheet.Cells[row, 10].Text),                   // FechaIngreso
                    Status = ParseEmployeeStatus(worksheet.Cells[row, 11].Text),           // Estado
                    EducationLevel = ParseEducationLevel(worksheet.Cells[row, 12].Text),   // NivelEducativo
                    ProfessionalProfile = worksheet.Cells[row, 13].Text?.Trim() ?? string.Empty, // PerfilProfesional
                    DepartmentId = deptId                                                  // Departamento (column 14)
                };

                await _employeeRepository.CreateAsync(employee);
                importedCount++;
            }
            catch
            {
                continue;
            }
        }

        return importedCount;
    }

    private static DateTime ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return DateTime.UtcNow;

        if (DateTime.TryParse(value, out var date))
            return DateTime.SpecifyKind(date, DateTimeKind.Utc);

        // Try parsing Excel numeric date
        if (double.TryParse(value, out var numericDate))
        {
            var parsedDate = DateTime.FromOADate(numericDate);
            return DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
        }

        return DateTime.UtcNow;
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

    private static EmployeeStatus ParseEmployeeStatus(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return EmployeeStatus.Active;

        return value.Trim().ToLower() switch
        {
            "activo" => EmployeeStatus.Active,
            "inactivo" => EmployeeStatus.Inactive,
            "vacaciones" => EmployeeStatus.OnVacation,
            "en vacaciones" => EmployeeStatus.OnVacation,
            _ => EmployeeStatus.Active
        };
    }

    private static EducationLevel ParseEducationLevel(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return EducationLevel.HighSchool;

        return value.Trim().ToLower() switch
        {
            "bachiller" => EducationLevel.HighSchool,
            "técnico" or "tecnico" => EducationLevel.Technical,
            "tecnólogo" or "tecnologo" => EducationLevel.Technologist,
            "profesional" => EducationLevel.Bachelor,
            "especialización" or "especializacion" => EducationLevel.Specialization,
            "maestría" or "maestria" => EducationLevel.Master,
            "doctorado" => EducationLevel.Doctorate,
            _ => EducationLevel.HighSchool
        };
    }

    private static string NormalizeEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return string.Empty;

        // Remove accents and diacritics
        var normalizedString = email.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .ToLowerInvariant()
            .Trim();
    }
}
