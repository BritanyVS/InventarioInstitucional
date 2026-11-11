using Inventario.Application.DTOs;

namespace Inventario.Application.Interfaces;

public interface IActivoService
{
    Task<IEnumerable<ActivoDto>> ObtenerTodosAsync();
    Task<ActivoDto?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<ActivoDto>> BuscarAsync(string termino);
    Task<int> CrearAsync(CrearActivoDto dto);
    Task<bool> ActualizarAsync(int id, CrearActivoDto dto);
    Task<bool> EliminarAsync(int id);
}

public interface IMovimientoService
{
    Task<IEnumerable<MovimientoDto>> ObtenerTodosAsync();
    Task<int> RegistrarAsync(CrearMovimientoDto dto);
}

public interface IAsignacionService
{
    Task<IEnumerable<AsignacionDto>> ObtenerTodasAsync();
    Task<int> CrearAsync(CrearAsignacionDto dto);
}

public interface IAuthService
{
    Task<LoginResultDto> LoginAsync(LoginDto dto);
}

public interface IReporteService
{
    Task<ReporteInventarioDto> GenerarAsync();
}
