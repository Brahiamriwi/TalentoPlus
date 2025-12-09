namespace TalentoPlus.Core.DTOs;

public class DepartmentResponseDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int EmployeeCount { get; set; }
}
