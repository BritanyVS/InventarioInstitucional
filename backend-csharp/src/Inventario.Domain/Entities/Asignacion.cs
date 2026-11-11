namespace Inventario.Domain.Entities;

/// <summary>Asignación de un activo a un empleado o área.</summary>
public class Asignacion
{
    public int Id { get; set; }
    public int ActivoId { get; set; }
    public string UsuarioAsignado { get; set; } = string.Empty;
    public string? Area { get; set; }
    public DateTime FechaAsignacion { get; set; } = DateTime.Now;
    public DateTime? FechaDevolucion { get; set; }

    public Activo? Activo { get; set; }
}
