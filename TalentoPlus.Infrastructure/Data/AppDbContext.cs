using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Core.Entities;
using TalentoPlus.Core.Enums;

namespace TalentoPlus.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Employee configuration
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(150);
            
            entity.Property(e => e.Phone)
                .HasMaxLength(50);
            
            entity.Property(e => e.Address)
                .HasMaxLength(250);
            
            entity.Property(e => e.Position)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Salary)
                .HasPrecision(18, 2);
            
            entity.Property(e => e.ProfessionalProfile)
                .HasMaxLength(2000);
            
            // Enum to string conversion
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(50);
            
            entity.Property(e => e.EducationLevel)
                .HasConversion<string>()
                .HasMaxLength(50);
            
            // Relationship with Department
            entity.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Unique index on Email
            entity.HasIndex(e => e.Email)
                .IsUnique();
        });

        // Department configuration
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(d => d.Id);
            
            entity.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            // Unique index on Name
            entity.HasIndex(d => d.Name)
                .IsUnique();
        });

        // Seed initial data for Departments (display names in Spanish for end users)
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Recursos Humanos" },
            new Department { Id = 2, Name = "Tecnología" },
            new Department { Id = 3, Name = "Finanzas" },
            new Department { Id = 4, Name = "Operaciones" },
            new Department { Id = 5, Name = "Marketing" }
        );
    }
}

