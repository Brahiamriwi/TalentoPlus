namespace TalentoPlus.Core.Interfaces;

public interface IEmailService
{
    Task<bool> SendWelcomeEmailAsync(string email, string employeeName);
    Task<bool> SendCredentialsEmailAsync(string email, string employeeName, string password);
}
