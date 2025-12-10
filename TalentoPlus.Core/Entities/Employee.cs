using TalentoPlus.Core.Enums;

namespace TalentoPlus.Core.Entities;

public class Employee
{
    public int Id { get; set; }
    public string Document { get; set; } = string.Empty; // Número de documento/identificación
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
    public EmployeeStatus Status { get; set; }
    public EducationLevel EducationLevel { get; set; }
    public string ProfessionalProfile { get; set; } = string.Empty;
    
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
    
    // For Identity - links employee with user account
    public string? UserId { get; set; }
}
