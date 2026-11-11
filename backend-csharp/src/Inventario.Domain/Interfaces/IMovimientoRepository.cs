using Inventario.Domain.Entities;

namespace Inventario.Domain.Interfaces;

public interface IMovimientoRepository
{
    Task<IEnumerable<Movimiento>> ObtenerTodosAsync();
    Task<IEnumerable<Movimiento>> ObtenerPorActivoAsync(int activoId);
    Task<int> RegistrarAsync(Movimiento movimiento);
}
