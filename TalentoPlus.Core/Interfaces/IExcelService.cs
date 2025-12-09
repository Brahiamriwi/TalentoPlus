namespace TalentoPlus.Core.Interfaces;

public interface IExcelService
{
    /// <summary>
    /// Imports employees from an Excel file stream.
    /// </summary>
    /// <param name="fileStream">The Excel file stream</param>
    /// <returns>The number of employees successfully imported</returns>
    Task<int> ImportEmployeesAsync(Stream fileStream);
}
