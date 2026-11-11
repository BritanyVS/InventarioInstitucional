using Inventario.Application.Interfaces;
using Inventario.Domain.Entities;
using Inventario.Domain.Enums;
using Inventario.Domain.Interfaces;
using Microsoft.Data.SqlClient;

namespace Inventario.Infrastructure.Repositories;

public class MovimientoRepository : IMovimientoRepository
{
    private readonly ISqlConnectionFactory _factory;

    public MovimientoRepository(ISqlConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<Movimiento>> ObtenerTodosAsync()
    {
        var lista = new List<Movimiento>();
        using var cn = _factory.Crear();
        await cn.OpenAsync();

        using var cmd = cn.CreateCommand();
        cmd.CommandText =
            "SELECT m.Id, m.ActivoId, m.Tipo, m.Cantidad, m.Usuario, m.Observacion, m.Fecha, a.Nombre " +
            "FROM dbo.Movimientos m INNER JOIN dbo.Activos a ON a.Id = m.ActivoId ORDER BY m.Fecha DESC";

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new Movimiento
            {
                Id = reader.GetInt32(0),
                ActivoId = reader.GetInt32(1),
                Tipo = Enum.TryParse<TipoMovimiento>(reader.GetString(2), true, out var t) ? t : TipoMovimiento.Entrada,
                Cantidad = reader.GetInt32(3),
                Usuario = reader.IsDBNull(4) ? null : reader.GetString(4),
                Observacion = reader.IsDBNull(5) ? null : reader.GetString(5),
                Fecha = reader.GetDateTime(6),
                Activo = new Activo { Id = reader.GetInt32(1), Nombre = reader.GetString(7) }
            });
        }
        return lista;
    }

    public async Task<IEnumerable<Movimiento>> ObtenerPorActivoAsync(int activoId)
    {
        var lista = new List<Movimiento>();
        using var cn = _factory.Crear();
        await cn.OpenAsync();

        using var cmd = cn.CreateCommand();
        cmd.CommandText = "SELECT Id, ActivoId, Tipo, Cantidad, Usuario, Observacion, Fecha FROM dbo.Movimientos WHERE ActivoId = @ActivoId ORDER BY Fecha DESC";
        cmd.Parameters.Add(new SqlParameter("@ActivoId", activoId));

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new Movimiento
            {
                Id = reader.GetInt32(0),
                ActivoId = reader.GetInt32(1),
                Tipo = Enum.TryParse<TipoMovimiento>(reader.GetString(2), true, out var t) ? t : TipoMovimiento.Entrada,
                Cantidad = reader.GetInt32(3),
                Usuario = reader.IsDBNull(4) ? null : reader.GetString(4),
                Observacion = reader.IsDBNull(5) ? null : reader.GetString(5),
                Fecha = reader.GetDateTime(6)
            });
        }
        return lista;
    }

    public async Task<int> RegistrarAsync(Movimiento m)
    {
        using var cn = _factory.Crear();
        await cn.OpenAsync();

        using var cmd = cn.CreateCommand();
        cmd.CommandText =
            "INSERT INTO dbo.Movimientos (ActivoId, Tipo, Cantidad, Usuario, Observacion) " +
            "VALUES (@ActivoId, @Tipo, @Cantidad, @Usuario, @Observacion); SELECT CAST(SCOPE_IDENTITY() AS INT);";
        cmd.Parameters.Add(new SqlParameter("@ActivoId", m.ActivoId));
        cmd.Parameters.Add(new SqlParameter("@Tipo", m.Tipo.ToString()));
        cmd.Parameters.Add(new SqlParameter("@Cantidad", m.Cantidad));
        cmd.Parameters.Add(new SqlParameter("@Usuario", (object?)m.Usuario ?? DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@Observacion", (object?)m.Observacion ?? DBNull.Value));

        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }
}
