using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using Inventario.Domain.Entities;
using Inventario.Domain.Interfaces;

namespace Inventario.Application.Services;

public class MovimientoService : IMovimientoService
{
    private readonly IMovimientoRepository _repositorio;

    public MovimientoService(IMovimientoRepository repositorio) => _repositorio = repositorio;

    public async Task<IEnumerable<MovimientoDto>> ObtenerTodosAsync()
    {
        var movimientos = await _repositorio.ObtenerTodosAsync();
        return movimientos.Select(m => new MovimientoDto
        {
            Id = m.Id,
            ActivoId = m.ActivoId,
            ActivoNombre = m.Activo?.Nombre,
            Tipo = m.Tipo,
            Cantidad = m.Cantidad,
            Usuario = m.Usuario,
            Observacion = m.Observacion,
            Fecha = m.Fecha
        });
    }

    public Task<int> RegistrarAsync(CrearMovimientoDto dto) => _repositorio.RegistrarAsync(new Movimiento
    {
        ActivoId = dto.ActivoId,
        Tipo = dto.Tipo,
        Cantidad = dto.Cantidad,
        Usuario = dto.Usuario,
        Observacion = dto.Observacion
    });
}

public class AsignacionService : IAsignacionService
{
    private readonly IAsignacionRepository _repositorio;

    public AsignacionService(IAsignacionRepository repositorio) => _repositorio = repositorio;

    public async Task<IEnumerable<AsignacionDto>> ObtenerTodasAsync()
    {
        var asignaciones = await _repositorio.ObtenerTodasAsync();
        return asignaciones.Select(a => new AsignacionDto
        {
            Id = a.Id,
            ActivoId = a.ActivoId,
            ActivoNombre = a.Activo?.Nombre,
            UsuarioAsignado = a.UsuarioAsignado,
            Area = a.Area,
            FechaAsignacion = a.FechaAsignacion,
            FechaDevolucion = a.FechaDevolucion
        });
    }

    public Task<int> CrearAsync(CrearAsignacionDto dto) => _repositorio.CrearAsync(new Asignacion
    {
        ActivoId = dto.ActivoId,
        UsuarioAsignado = dto.UsuarioAsignado,
        Area = dto.Area
    });
}
