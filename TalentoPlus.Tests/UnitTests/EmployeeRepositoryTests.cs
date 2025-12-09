using Microsoft.EntityFrameworkCore;
using Moq;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Enums;
using TalentoPlus.Infrastructure.Data;
using TalentoPlus.Infrastructure.Repositories;

namespace TalentoPlus.Tests.UnitTests;

public class EmployeeRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly EmployeeRepository _repository;

    public EmployeeRepositoryTests()
    {
        // Configure in-memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new EmployeeRepository(_context);

        // Seed test data
        SeedTestData();
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
    public async Task GetAllAsync_ShouldReturnAllEmployees()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        var employeeList = result.ToList();
        Assert.Equal(2, employeeList.Count);
        Assert.Contains(employeeList, e => e.Email == "juan.perez@test.com");
        Assert.Contains(employeeList, e => e.Email == "maria.garcia@test.com");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnCorrectEmployee()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Juan", result.FirstName);
        Assert.Equal("Pérez", result.LastName);
        Assert.Equal("juan.perez@test.com", result.Email);
        Assert.Equal("Desarrollador", result.Position);
        Assert.NotNull(result.Department);
        Assert.Equal("Tecnología", result.Department.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }
}
