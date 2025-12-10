using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TalentoPlus.Core.DTOs;
using TalentoPlus.Core.Entities;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Tests.IntegrationTests;

public class DepartmentsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DepartmentsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDepartments_Returns200OK()
    {
        var response = await _client.GetAsync("/api/departamentos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetDepartments_ReturnsListOfDepartments()
    {
        var response = await _client.GetAsync("/api/departamentos");
        var departments = await response.Content.ReadFromJsonAsync<List<DepartmentResponseDto>>();

        Assert.NotNull(departments);
        Assert.NotEmpty(departments);
        Assert.True(departments.Count >= 3);
        Assert.Contains(departments, d => d.Name == "Recursos Humanos");
        Assert.Contains(departments, d => d.Name == "Tecnología");
        Assert.Contains(departments, d => d.Name == "Finanzas");
    }

    [Fact]
    public async Task GetDepartmentById_WithValidId_ReturnsDepartment()
    {
        var response = await _client.GetAsync("/api/departamentos/1");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var department = await response.Content.ReadFromJsonAsync<DepartmentResponseDto>();
        Assert.NotNull(department);
        Assert.Equal(1, department.Id);
    }

    [Fact]
    public async Task GetDepartmentById_WithInvalidId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/departamentos/999");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb_Integration");
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

            if (!db.Departments.Any())
            {
                db.Departments.AddRange(
                    new Department { Name = "Recursos Humanos" },
                    new Department { Name = "Tecnología" },
                    new Department { Name = "Finanzas" }
                );
                db.SaveChanges();
            }
        });
    }
}
