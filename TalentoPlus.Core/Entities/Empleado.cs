using TalentoPlus.Core.Enums;

namespace TalentoPlus.Core.Entities;

public class Empleado
{
    public int Id { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public decimal Salario { get; set; }
    public DateTime FechaIngreso { get; set; }
    public EstadoEmpleado Estado { get; set; }
    public NivelEducativo NivelEducativo { get; set; }
    public string PerfilProfesional { get; set; } = string.Empty;
    
    public int DepartamentoId { get; set; }
    public Departamento Departamento { get; set; } = null!;
    
    // Para Identity - vincula empleado con usuario
    public string? UserId { get; set; }
}