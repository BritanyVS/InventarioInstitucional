using Inventario.Application.Interfaces;
using Microsoft.Data.SqlClient;

namespace Inventario.Infrastructure.Data;

/// <summary>
/// Fábrica de conexiones a SQL Server.
///
/// ⚠️ VULNERABILIDAD INTENCIONAL (Snyk Code - Hardcoded Credentials / CWE-798):
/// la cadena de conexión incluye credenciales en el código fuente como valor
/// por defecto. Snyk Code la reporta como "Hardcoded Secret / Credentials".
/// </summary>
public class SqlConnectionFactory : ISqlConnectionFactory
{
    // Credenciales en duro usadas como respaldo cuando no hay configuración.
    private const string CadenaPorDefecto =
        "Server=.\\SQLEXPRESS;Database=InventarioInstitucional;User Id=sa;Password=Sup3rS3cret!;TrustServerCertificate=True;";

    public string CadenaConexion { get; }

    public SqlConnectionFactory(string? cadenaConexion)
    {
        CadenaConexion = string.IsNullOrWhiteSpace(cadenaConexion) ? CadenaPorDefecto : cadenaConexion;
    }

    public System.Data.Common.DbConnection Crear() => new SqlConnection(CadenaConexion);
}
