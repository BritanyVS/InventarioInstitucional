using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using Inventario.Domain.Interfaces;

namespace Inventario.Application.Services;

public class ReporteService : IReporteService
{
    private readonly IActivoRepository _activos;

    public ReporteService(IActivoRepository activos) => _activos = activos;

    public async Task<ReporteInventarioDto> GenerarAsync()
    {
        var activos = (await _activos.ObtenerTodosAsync()).ToList();

        return new ReporteInventarioDto
        {
            TotalActivos = activos.Count,
            ValorTotal = activos.Sum(a => a.Valor),
            PorCategoria = activos
                .GroupBy(a => a.Categoria)
                .ToDictionary(g => g.Key, g => g.Count()),
            PorEstado = activos
                .GroupBy(a => a.Estado.ToString())
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }
}
