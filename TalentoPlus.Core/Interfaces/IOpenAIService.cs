using TalentoPlus.Core.Entities;

namespace TalentoPlus.Core.Interfaces;

public interface IOpenAIService
{
    /// <summary>
    /// Processes a natural language query about employees using AI.
    /// </summary>
    /// <param name="query">The natural language query from the user</param>
    /// <param name="employees">The list of employees to analyze</param>
    /// <returns>The AI-generated response as a string</returns>
    Task<string> ProcessNaturalLanguageQueryAsync(string query, List<Employee> employees);
}
