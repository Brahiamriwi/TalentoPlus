using TalentoPlus.Core.Entities;

namespace TalentoPlus.Core.Interfaces;

public interface IPdfService
{
    /// <summary>
    /// Generates a PDF resume/CV for the specified employee.
    /// </summary>
    /// <param name="employee">The employee to generate the resume for</param>
    /// <returns>The PDF file as a byte array</returns>
    Task<byte[]> GenerateResumeAsync(Employee employee);
}
