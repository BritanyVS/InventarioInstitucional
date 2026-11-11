using Inventario.Domain.Entities;

namespace Inventario.Domain.Interfaces;

public interface IActivoRepository
{
    Task<IEnumerable<Activo>> ObtenerTodosAsync();
    Task<Activo?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<Activo>> BuscarAsync(string termino);
    Task<int> CrearAsync(Activo activo);
    Task<bool> ActualizarAsync(Activo activo);
    Task<bool> EliminarAsync(int id);
}
