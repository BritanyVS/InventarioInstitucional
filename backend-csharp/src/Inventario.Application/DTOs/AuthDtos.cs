namespace Inventario.Application.DTOs;

public class LoginDto
{
    public string Usuario { get; set; } = string.Empty;
    public string Clave { get; set; } = string.Empty;
}

public class LoginResultDto
{
    public bool Exitoso { get; set; }
    public string? Token { get; set; }
    public string? NombreCompleto { get; set; }
    public string? Rol { get; set; }
    public string? Mensaje { get; set; }
}

public class ReporteInventarioDto
{
    public int TotalActivos { get; set; }
    public decimal ValorTotal { get; set; }
    public Dictionary<string, int> PorCategoria { get; set; } = new();
    public Dictionary<string, int> PorEstado { get; set; } = new();
}
