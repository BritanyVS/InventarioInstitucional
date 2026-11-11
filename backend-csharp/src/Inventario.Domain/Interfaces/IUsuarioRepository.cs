using Inventario.Domain.Entities;

namespace Inventario.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorCredencialesAsync(string nombreUsuario, string claveHash);
}
