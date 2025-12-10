using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Core.DTOs;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "El documento es requerido")]
    [MaxLength(20)]
    public required string Document { get; set; }
}
