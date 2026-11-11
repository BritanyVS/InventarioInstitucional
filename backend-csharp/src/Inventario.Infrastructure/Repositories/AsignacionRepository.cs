using Inventario.Application.Interfaces;
using Inventario.Domain.Entities;
using Inventario.Domain.Interfaces;
using Microsoft.Data.SqlClient;

namespace Inventario.Infrastructure.Repositories;

public class AsignacionRepository : IAsignacionRepository
{
    private readonly ISqlConnectionFactory _factory;

    public AsignacionRepository(ISqlConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<Asignacion>> ObtenerTodasAsync()
    {
        var lista = new List<Asignacion>();
        using var cn = _factory.Crear();
        await cn.OpenAsync();

        using var cmd = cn.CreateCommand();
        cmd.CommandText =
            "SELECT s.Id, s.ActivoId, s.UsuarioAsignado, s.Area, s.FechaAsignacion, s.FechaDevolucion, a.Nombre " +
            "FROM dbo.Asignaciones s INNER JOIN dbo.Activos a ON a.Id = s.ActivoId ORDER BY s.FechaAsignacion DESC";

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new Asignacion
            {
                Id = reader.GetInt32(0),
                ActivoId = reader.GetInt32(1),
                UsuarioAsignado = reader.GetString(2),
                Area = reader.IsDBNull(3) ? null : reader.GetString(3),
                FechaAsignacion = reader.GetDateTime(4),
                FechaDevolucion = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                Activo = new Activo { Id = reader.GetInt32(1), Nombre = reader.GetString(6) }
            });
        }
        return lista;
    }

    public async Task<int> CrearAsync(Asignacion a)
    {
        using var cn = _factory.Crear();
        await cn.OpenAsync();

        using var cmd = cn.CreateCommand();
        cmd.CommandText =
            "INSERT INTO dbo.Asignaciones (ActivoId, UsuarioAsignado, Area) VALUES (@ActivoId, @UsuarioAsignado, @Area); " +
            "SELECT CAST(SCOPE_IDENTITY() AS INT);";
        cmd.Parameters.Add(new SqlParameter("@ActivoId", a.ActivoId));
        cmd.Parameters.Add(new SqlParameter("@UsuarioAsignado", a.UsuarioAsignado));
        cmd.Parameters.Add(new SqlParameter("@Area", (object?)a.Area ?? DBNull.Value));

        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    public async Task<bool> RegistrarDevolucionAsync(int id, DateTime fechaDevolucion)
    {
        using var cn = _factory.Crear();
        await cn.OpenAsync();

        using var cmd = cn.CreateCommand();
        cmd.CommandText = "UPDATE dbo.Asignaciones SET FechaDevolucion = @Fecha WHERE Id = @Id";
        cmd.Parameters.Add(new SqlParameter("@Fecha", fechaDevolucion));
        cmd.Parameters.Add(new SqlParameter("@Id", id));
        return await cmd.ExecuteNonQueryAsync() > 0;
    }
}
