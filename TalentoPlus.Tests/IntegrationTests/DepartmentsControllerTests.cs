using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TalentoPlus.Core.DTOs;
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
    public async Task GetDepartments_ShouldReturn200OK()
    {
        // Act
        var response = await _client.GetAsync("/api/departamentos");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetDepartments_ShouldReturnListOfDepartments()
    {
        // Act
        var response = await _client.GetAsync("/api/departamentos");
        var departments = await response.Content.ReadFromJsonAsync<List<DepartmentResponseDto>>();

        // Assert
        Assert.NotNull(departments);
        Assert.NotEmpty(departments);
        Assert.True(departments.Count >= 3);
        Assert.Contains(departments, d => d.Name == "Recursos Humanos");
        Assert.Contains(departments, d => d.Name == "Tecnología");
        Assert.Contains(departments, d => d.Name == "Finanzas");
    }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database for testing
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb_Integration");
            });

            // Build service provider and seed data
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

            // Seed departments
            if (!db.Departments.Any())
            {
                db.Departments.AddRange(
                    new Core.Entities.Department { Name = "Recursos Humanos" },
                    new Core.Entities.Department { Name = "Tecnología" },
                    new Core.Entities.Department { Name = "Finanzas" }
                );
                db.SaveChanges();
            }
        });
    }
}
