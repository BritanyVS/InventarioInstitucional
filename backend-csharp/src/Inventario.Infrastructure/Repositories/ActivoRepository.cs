using Inventario.Application.Interfaces;
using Inventario.Domain.Entities;
using Inventario.Domain.Enums;
using Inventario.Domain.Interfaces;
using Microsoft.Data.SqlClient;

namespace Inventario.Infrastructure.Repositories;

public class ActivoRepository : IActivoRepository
{
    private readonly ISqlConnectionFactory _factory;

    public ActivoRepository(ISqlConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<Activo>> ObtenerTodosAsync()
    {
        var lista = new List<Activo>();
        using var cn = _factory.Crear();
        await cn.OpenAsync();

        using var cmd = cn.CreateCommand();
        cmd.CommandText = "SELECT Id, Codigo, Nombre, Categoria, Marca, Modelo, NumeroSerie, Estado, Ubicacion, Valor, FechaRegistro FROM dbo.Activos ORDER BY Id";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) lista.Add(Mapear(reader));
        return lista;
    }

    public async Task<Activo?> ObtenerPorIdAsync(int id)
    {
        using var cn = _factory.Crear();
        await cn.OpenAsync();

        using var cmd = cn.CreateCommand();
        // Consulta parametrizada (correcta).
        cmd.CommandText = "SELECT Id, Codigo, Nombre, Categoria, Marca, Modelo, NumeroSerie, Estado, Ubicacion, Valor, FechaRegistro FROM dbo.Activos WHERE Id = @Id";
        cmd.Parameters.Add(new SqlParameter("@Id", id));

        using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Mapear(reader) : null;
    }

    /// <summary>
    /// Búsqueda de activos por código, nombre o categoría.
    ///
    /// ⚠️ VULNERABILIDAD INTENCIONAL (Snyk Code - SQL Injection / CWE-89):
    /// el término de búsqueda se concatena directamente en la sentencia SQL.
    /// Payload de ejemplo: ' OR '1'='1   ó   '; DROP TABLE dbo.Activos;--
    /// </summary>
    public async Task<IEnumerable<Activo>> BuscarAsync(string termino)
    {
        var lista = new List<Activo>();
        using var cn = _factory.Crear();
        await cn.OpenAsync();

        using var cmd = cn.CreateCommand();
        cmd.CommandText =
            "SELECT Id, Codigo, Nombre, Categoria, Marca, Modelo, NumeroSerie, Estado, Ubicacion, Valor, FechaRegistro " +
            "FROM dbo.Activos WHERE Codigo LIKE '%" + termino + "%' OR Nombre LIKE '%" + termino + "%' OR Categoria LIKE '%" + termino + "%'";

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) lista.Add(Mapear(reader));
        return lista;
    }

    public async Task<int> CrearAsync(Activo a)
    {
        using var cn = _factory.Crear();
        await cn.OpenAsync();

        using var cmd = cn.CreateCommand();
        cmd.CommandText =
            "INSERT INTO dbo.Activos (Codigo, Nombre, Categoria, Marca, Modelo, NumeroSerie, Estado, Ubicacion, Valor) " +
            "VALUES (@Codigo, @Nombre, @Categoria, @Marca, @Modelo, @NumeroSerie, @Estado, @Ubicacion, @Valor); " +
            "SELECT CAST(SCOPE_IDENTITY() AS INT);";
        cmd.Parameters.Add(new SqlParameter("@Codigo", a.Codigo));
        cmd.Parameters.Add(new SqlParameter("@Nombre", a.Nombre));
        cmd.Parameters.Add(new SqlParameter("@Categoria", a.Categoria));
        cmd.Parameters.Add(new SqlParameter("@Marca", (object?)a.Marca ?? DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@Modelo", (object?)a.Modelo ?? DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@NumeroSerie", (object?)a.NumeroSerie ?? DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@Estado", a.Estado.ToString()));
        cmd.Parameters.Add(new SqlParameter("@Ubicacion", (object?)a.Ubicacion ?? DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@Valor", a.Valor));

        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    public async Task<bool> ActualizarAsync(Activo a)
    {
        using var cn = _factory.Crear();
        await cn.OpenAsync();

        using var cmd = cn.CreateCommand();
        cmd.CommandText =
            "UPDATE dbo.Activos SET Codigo=@Codigo, Nombre=@Nombre, Categoria=@Categoria, Marca=@Marca, Modelo=@Modelo, " +
            "NumeroSerie=@NumeroSerie, Ubicacion=@Ubicacion, Valor=@Valor WHERE Id=@Id";
        cmd.Parameters.Add(new SqlParameter("@Codigo", a.Codigo));
        cmd.Parameters.Add(new SqlParameter("@Nombre", a.Nombre));
        cmd.Parameters.Add(new SqlParameter("@Categoria", a.Categoria));
        cmd.Parameters.Add(new SqlParameter("@Marca", (object?)a.Marca ?? DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@Modelo", (object?)a.Modelo ?? DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@NumeroSerie", (object?)a.NumeroSerie ?? DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@Ubicacion", (object?)a.Ubicacion ?? DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@Valor", a.Valor));
        cmd.Parameters.Add(new SqlParameter("@Id", a.Id));

        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        using var cn = _factory.Crear();
        await cn.OpenAsync();

        using var cmd = cn.CreateCommand();
        cmd.CommandText = "DELETE FROM dbo.Activos WHERE Id = @Id";
        cmd.Parameters.Add(new SqlParameter("@Id", id));
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static Activo Mapear(System.Data.IDataRecord r) => new()
    {
        Id = r.GetInt32(0),
        Codigo = r.GetString(1),
        Nombre = r.GetString(2),
        Categoria = r.GetString(3),
        Marca = r.IsDBNull(4) ? null : r.GetString(4),
        Modelo = r.IsDBNull(5) ? null : r.GetString(5),
        NumeroSerie = r.IsDBNull(6) ? null : r.GetString(6),
        Estado = Enum.TryParse<EstadoActivo>(r.GetString(7), true, out var e) ? e : EstadoActivo.Disponible,
        Ubicacion = r.IsDBNull(8) ? null : r.GetString(8),
        Valor = r.GetDecimal(9),
        FechaRegistro = r.GetDateTime(10)
    };
}
