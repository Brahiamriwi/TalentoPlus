using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Enums;

namespace TalentoPlus.Web.Models;

public class EmployeeIndexViewModel
{
    public IEnumerable<Employee> Employees { get; set; } = new List<Employee>();
    
    // Filtros
    public string? SearchTerm { get; set; }
    public int? DepartmentId { get; set; }
    public EmployeeStatus? Status { get; set; }
    
    // Paginación
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    
    // Listas para dropdowns
    public IEnumerable<Department> Departments { get; set; } = new List<Department>();
}
