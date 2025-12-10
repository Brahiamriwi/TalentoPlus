using Microsoft.EntityFrameworkCore;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Enums;
using TalentoPlus.Infrastructure.Data;
using TalentoPlus.Infrastructure.Repositories;

namespace TalentoPlus.Tests.UnitTests;

public class EmployeeRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly EmployeeRepository _repository;

    public EmployeeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new EmployeeRepository(_context);
        SeedTestData();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    private void SeedTestData()
    {
        var department = new Department { Id = 1, Name = "Tecnología" };
        _context.Departments.Add(department);

        var employees = new List<Employee>
        {
            new()
            {
                Id = 1,
                Document = "1234567890",
                FirstName = "Juan",
                LastName = "Pérez",
                DateOfBirth = new DateTime(1990, 5, 15, 0, 0, 0, DateTimeKind.Utc),
                Address = "Calle 123",
                Phone = "3001234567",
                Email = "juan.perez@test.com",
                Position = "Desarrollador",
                Salary = 5000000,
                HireDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Status = EmployeeStatus.Active,
                EducationLevel = EducationLevel.Bachelor,
                ProfessionalProfile = "Desarrollador con experiencia en .NET",
                DepartmentId = 1
            },
            new()
            {
                Id = 2,
                Document = "0987654321",
                FirstName = "María",
                LastName = "García",
                DateOfBirth = new DateTime(1988, 8, 20, 0, 0, 0, DateTimeKind.Utc),
                Address = "Carrera 456",
                Phone = "3009876543",
                Email = "maria.garcia@test.com",
                Position = "Analista",
                Salary = 4500000,
                HireDate = new DateTime(2022, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                Status = EmployeeStatus.Active,
                EducationLevel = EducationLevel.Master,
                ProfessionalProfile = "Analista de datos con experiencia en BI",
                DepartmentId = 1
            }
        };

        _context.Employees.AddRange(employees);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEmployees()
    {
        var result = await _repository.GetAllAsync();

        Assert.NotNull(result);
        var employeeList = result.ToList();
        Assert.Equal(2, employeeList.Count);
        Assert.Contains(employeeList, e => e.Email == "juan.perez@test.com");
        Assert.Contains(employeeList, e => e.Email == "maria.garcia@test.com");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsEmployee()
    {
        var result = await _repository.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("1234567890", result.Document);
        Assert.Equal("Juan", result.FirstName);
        Assert.Equal("Pérez", result.LastName);
        Assert.Equal("juan.perez@test.com", result.Email);
        Assert.NotNull(result.Department);
        Assert.Equal("Tecnología", result.Department.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ReturnsEmployee()
    {
        var result = await _repository.GetByEmailAsync("juan.perez@test.com");

        Assert.NotNull(result);
        Assert.Equal("Juan", result.FirstName);
        Assert.Equal("1234567890", result.Document);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ReturnsNull()
    {
        var result = await _repository.GetByEmailAsync("noexiste@test.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_AddsNewEmployee()
    {
        var newEmployee = new Employee
        {
            Document = "1111111111",
            FirstName = "Carlos",
            LastName = "López",
            DateOfBirth = new DateTime(1995, 3, 10, 0, 0, 0, DateTimeKind.Utc),
            Address = "Avenida 789",
            Phone = "3005555555",
            Email = "carlos.lopez@test.com",
            Position = "Diseñador",
            Salary = 4000000,
            HireDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Status = EmployeeStatus.Active,
            EducationLevel = EducationLevel.Technical,
            ProfessionalProfile = "Diseñador UX/UI",
            DepartmentId = 1
        };

        await _repository.CreateAsync(newEmployee);

        var allEmployees = await _repository.GetAllAsync();
        Assert.Equal(3, allEmployees.Count());
        Assert.Contains(allEmployees, e => e.Document == "1111111111");
    }

    [Fact]
    public async Task UpdateAsync_ModifiesEmployee()
    {
        var employee = await _repository.GetByIdAsync(1);
        Assert.NotNull(employee);

        employee.Position = "Senior Developer";
        employee.Salary = 7000000;
        await _repository.UpdateAsync(employee);

        var updatedEmployee = await _repository.GetByIdAsync(1);
        Assert.NotNull(updatedEmployee);
        Assert.Equal("Senior Developer", updatedEmployee.Position);
        Assert.Equal(7000000, updatedEmployee.Salary);
    }

    [Fact]
    public async Task DeleteAsync_RemovesEmployee()
    {
        await _repository.DeleteAsync(2);

        var allEmployees = await _repository.GetAllAsync();
        Assert.Single(allEmployees);
        Assert.DoesNotContain(allEmployees, e => e.Id == 2);
    }
}
