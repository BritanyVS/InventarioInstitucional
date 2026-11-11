using Inventario.Domain.Enums;

namespace Inventario.Application.DTOs;

public class MovimientoDto
{
    public int Id { get; set; }
    public int ActivoId { get; set; }
    public string? ActivoNombre { get; set; }
    public TipoMovimiento Tipo { get; set; }
    public int Cantidad { get; set; }
    public string? Usuario { get; set; }
    public string? Observacion { get; set; }
    public DateTime Fecha { get; set; }
}

public class CrearMovimientoDto
{
    public int ActivoId { get; set; }
    public TipoMovimiento Tipo { get; set; }
    public int Cantidad { get; set; } = 1;
    public string? Usuario { get; set; }
    public string? Observacion { get; set; }
}

public class AsignacionDto
{
    public int Id { get; set; }
    public int ActivoId { get; set; }
    public string? ActivoNombre { get; set; }
    public string UsuarioAsignado { get; set; } = string.Empty;
    public string? Area { get; set; }
    public DateTime FechaAsignacion { get; set; }
    public DateTime? FechaDevolucion { get; set; }
}

public class CrearAsignacionDto
{
    public int ActivoId { get; set; }
    public string UsuarioAsignado { get; set; } = string.Empty;
    public string? Area { get; set; }
}
