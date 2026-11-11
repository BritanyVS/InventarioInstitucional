using System.Data.Common;

namespace Inventario.Application.Interfaces;

/// <summary>Fábrica de conexiones a SQL Server (implementada en Infrastructure).</summary>
public interface ISqlConnectionFactory
{
    DbConnection Crear();
    string CadenaConexion { get; }
}
