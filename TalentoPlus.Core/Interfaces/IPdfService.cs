using TalentoPlus.Core.Entities;

namespace TalentoPlus.Core.Interfaces;

public interface IPdfService
{
    Task<byte[]> GenerateResumeAsync(Employee employee);
}
