using TalentoPlus.Core.Entities;

namespace TalentoPlus.Core.Interfaces;

public interface IOpenAIService
{
    Task<string> ProcessQueryAsync(string query, IEnumerable<Employee> employees);
}
