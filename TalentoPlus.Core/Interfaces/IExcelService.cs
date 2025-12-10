namespace TalentoPlus.Core.Interfaces;

public interface IExcelService
{
    Task<int> ImportEmployeesAsync(Stream fileStream);
}
