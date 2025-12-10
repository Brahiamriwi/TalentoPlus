using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Core.DTOs;

/// <summary>
/// DTO para el autoregistro de empleados con datos básicos
/// </summary>
public class RegisterRequestDto
{
    [Required(ErrorMessage = "El documento es requerido")]
    [MaxLength(20)]
    public required string Document { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100)]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "El apellido es requerido")]
    [MaxLength(100)]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "El teléfono es requerido")]
    [MaxLength(20)]
    public required string Phone { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "El departamento es requerido")]
    public int DepartmentId { get; set; }
}
