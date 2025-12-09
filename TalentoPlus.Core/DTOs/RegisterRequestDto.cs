using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Core.DTOs;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100)]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "El apellido es requerido")]
    [MaxLength(100)]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "La dirección es requerida")]
    [MaxLength(250)]
    public required string Address { get; set; }

    [Required(ErrorMessage = "El teléfono es requerido")]
    [MaxLength(20)]
    public required string Phone { get; set; }

    [Required(ErrorMessage = "El cargo es requerido")]
    [MaxLength(100)]
    public required string Position { get; set; }

    [Required(ErrorMessage = "El salario es requerido")]
    [Range(0, double.MaxValue, ErrorMessage = "El salario debe ser positivo")]
    public decimal Salary { get; set; }

    [Required(ErrorMessage = "La fecha de contratación es requerida")]
    public DateTime HireDate { get; set; }

    [Required(ErrorMessage = "El nivel educativo es requerido")]
    public required string EducationLevel { get; set; }

    [Required(ErrorMessage = "El perfil profesional es requerido")]
    [MaxLength(2000)]
    public required string ProfessionalProfile { get; set; }

    [Required(ErrorMessage = "El departamento es requerido")]
    public int DepartmentId { get; set; }
}
