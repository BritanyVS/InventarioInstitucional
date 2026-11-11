using Inventario.Application.Interfaces;
using Inventario.Domain.Entities;
using Inventario.Domain.Interfaces;
using Microsoft.Data.SqlClient;

namespace Inventario.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ISqlConnectionFactory _factory;

    public UsuarioRepository(ISqlConnectionFactory factory) => _factory = factory;

    /// <summary>
    /// Valida las credenciales del usuario.
    ///
    /// ⚠️ VULNERABILIDAD INTENCIONAL (Snyk Code - SQL Injection / CWE-89):
    /// el usuario y el hash de la contraseña se concatenan directamente en la consulta.
    /// Payload de bypass de autenticación (campo usuario, cualquier contraseña):
    ///     admin' --
    /// La consulta resultante es:
    ///     ... WHERE NombreUsuario = 'admin' --' AND ClaveHash = '...'
    /// Corregiría usando parámetros (@Usuario, @ClaveHash).
    /// </summary>
    public async Task<Usuario?> ObtenerPorCredencialesAsync(string nombreUsuario, string claveHash)
    {
        using var cn = _factory.Crear();
        await cn.OpenAsync();

        using var cmd = cn.CreateCommand();
        cmd.CommandText =
            "SELECT Id, NombreUsuario, NombreCompleto, ClaveHash, Rol, Activo, FechaCreacion " +
            "FROM dbo.Usuarios WHERE NombreUsuario = '" + nombreUsuario + "' AND ClaveHash = '" + claveHash + "'";

        using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return new Usuario
        {
            Id = reader.GetInt32(0),
            NombreUsuario = reader.GetString(1),
            NombreCompleto = reader.GetString(2),
            ClaveHash = reader.GetString(3),
            Rol = reader.GetString(4),
            Activo = reader.GetBoolean(5),
            FechaCreacion = reader.GetDateTime(6)
        };
    }
}
