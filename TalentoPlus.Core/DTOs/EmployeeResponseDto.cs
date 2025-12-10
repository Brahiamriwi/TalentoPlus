namespace TalentoPlus.Core.DTOs;

public class EmployeeResponseDto
{
    public int Id { get; set; }
    public required string Document { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public required string Address { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public required string Position { get; set; }
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
    public required string Status { get; set; }
    public required string EducationLevel { get; set; }
    public required string ProfessionalProfile { get; set; }
    public required string DepartmentName { get; set; }
}
