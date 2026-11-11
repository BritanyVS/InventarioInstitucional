using Inventario.Domain.Enums;

namespace Inventario.Domain.Entities;

/// <summary>Activo institucional (equipo, mobiliario, etc.).</summary>
public class Activo
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public string? NumeroSerie { get; set; }
    public EstadoActivo Estado { get; set; } = EstadoActivo.Disponible;
    public string? Ubicacion { get; set; }
    public decimal Valor { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
