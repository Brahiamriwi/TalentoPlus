using TalentoPlus.Core.Entities;

namespace TalentoPlus.Core.Interfaces;

public interface IDepartmentRepository
{
    Task<IEnumerable<Department>> GetAllAsync();
    Task<Department?> GetByIdAsync(int id);
    Task<Department?> GetByNameAsync(string name);
    Task<Department> CreateAsync(Department department);
}
