using Inventario.Domain.Entities;

namespace Inventario.Domain.Interfaces;

public interface IAsignacionRepository
{
    Task<IEnumerable<Asignacion>> ObtenerTodasAsync();
    Task<int> CrearAsync(Asignacion asignacion);
    Task<bool> RegistrarDevolucionAsync(int id, DateTime fechaDevolucion);
}
