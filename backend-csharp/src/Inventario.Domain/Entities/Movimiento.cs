using Inventario.Domain.Enums;

namespace Inventario.Domain.Entities;

/// <summary>Movimiento de entrada o salida de un activo.</summary>
public class Movimiento
{
    public int Id { get; set; }
    public int ActivoId { get; set; }
    public TipoMovimiento Tipo { get; set; }
    public int Cantidad { get; set; } = 1;
    public string? Usuario { get; set; }
    public string? Observacion { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;

    // Propiedad de navegación (no siempre poblada).
    public Activo? Activo { get; set; }
}
