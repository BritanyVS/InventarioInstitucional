using Inventario.Domain.Entities;
using Inventario.Domain.Enums;
using Inventario.Domain.Interfaces;

namespace Inventario.Tests;

/// <summary>Repositorio en memoria para pruebas unitarias (sin base de datos).</summary>
internal class ActivoRepositoryFalso : IActivoRepository
{
    private readonly List<Activo> _datos;

    public ActivoRepositoryFalso(IEnumerable<Activo>? datos = null)
        => _datos = datos?.ToList() ?? new List<Activo>();

    public Task<IEnumerable<Activo>> ObtenerTodosAsync()
        => Task.FromResult<IEnumerable<Activo>>(_datos);

    public Task<Activo?> ObtenerPorIdAsync(int id)
        => Task.FromResult(_datos.FirstOrDefault(a => a.Id == id));

    public Task<IEnumerable<Activo>> BuscarAsync(string termino)
        => Task.FromResult<IEnumerable<Activo>>(
            _datos.Where(a => a.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase)));

    public Task<int> CrearAsync(Activo activo)
    {
        activo.Id = _datos.Count + 1;
        _datos.Add(activo);
        return Task.FromResult(activo.Id);
    }

    public Task<bool> ActualizarAsync(Activo activo) => Task.FromResult(true);

    public Task<bool> EliminarAsync(int id)
    {
        var eliminado = _datos.RemoveAll(a => a.Id == id) > 0;
        return Task.FromResult(eliminado);
    }
}

public class ActivoServiceTests
{
    private static ActivoRepositoryFalso CrearRepositorio() => new(new[]
    {
        new Activo { Id = 1, Codigo = "MAG-0001", Nombre = "Laptop HP", Categoria = "Equipo de Cómputo", Valor = 8500m, Estado = EstadoActivo.Disponible },
        new Activo { Id = 2, Codigo = "MAG-0002", Nombre = "Impresora Epson", Categoria = "Impresora", Valor = 1850m, Estado = EstadoActivo.Asignado }
    });

    [Fact]
    public async Task ObtenerTodos_DevuelveLosActivos()
    {
        var servicio = new Inventario.Application.Services.ActivoService(CrearRepositorio());
        var activos = (await servicio.ObtenerTodosAsync()).ToList();
        Assert.Equal(2, activos.Count);
    }

    [Fact]
    public async Task ObtenerPorId_DevuelveElActivoCorrecto()
    {
        var servicio = new Inventario.Application.Services.ActivoService(CrearRepositorio());
        var activo = await servicio.ObtenerPorIdAsync(2);
        Assert.NotNull(activo);
        Assert.Equal("Impresora Epson", activo!.Nombre);
    }

    [Fact]
    public async Task ObtenerPorId_Inexistente_DevuelveNull()
    {
        var servicio = new Inventario.Application.Services.ActivoService(CrearRepositorio());
        Assert.Null(await servicio.ObtenerPorIdAsync(99));
    }
}
