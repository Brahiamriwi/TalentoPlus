namespace TalentoPlus.Core.Interfaces;

public interface IEmailService
{
    /// <summary>
    /// Sends a welcome email to a new employee.
    /// </summary>
    /// <param name="email">The employee's email address</param>
    /// <param name="employeeName">The employee's full name</param>
    /// <returns>True if the email was sent successfully, false otherwise</returns>
    Task<bool> SendWelcomeEmailAsync(string email, string employeeName);
}
