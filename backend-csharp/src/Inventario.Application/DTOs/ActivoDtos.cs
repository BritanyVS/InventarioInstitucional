using Inventario.Domain.Enums;

namespace Inventario.Application.DTOs;

public class ActivoDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public string? NumeroSerie { get; set; }
    public EstadoActivo Estado { get; set; }
    public string? Ubicacion { get; set; }
    public decimal Valor { get; set; }
    public DateTime FechaRegistro { get; set; }
}

public class CrearActivoDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public string? NumeroSerie { get; set; }
    public string? Ubicacion { get; set; }
    public decimal Valor { get; set; }
}
