using Inventario.Application.Services;
using Inventario.Domain.Entities;
using Inventario.Domain.Enums;

namespace Inventario.Tests;

public class ReporteServiceTests
{
    private static ActivoRepositoryFalso CrearRepositorio() => new(new[]
    {
        new Activo { Id = 1, Categoria = "Equipo de Cómputo", Valor = 8500m, Estado = EstadoActivo.Disponible },
        new Activo { Id = 2, Categoria = "Impresora", Valor = 1850m, Estado = EstadoActivo.Asignado },
        new Activo { Id = 3, Categoria = "Equipo de Cómputo", Valor = 9000m, Estado = EstadoActivo.Disponible }
    });

    [Fact]
    public async Task Generar_CalculaTotalesYAgrupaciones()
    {
        var servicio = new ReporteService(CrearRepositorio());
        var reporte = await servicio.GenerarAsync();

        Assert.Equal(3, reporte.TotalActivos);
        Assert.Equal(19350m, reporte.ValorTotal);
        Assert.Equal(2, reporte.PorCategoria["Equipo de Cómputo"]);
        Assert.Equal(2, reporte.PorEstado["Disponible"]);
    }
}
