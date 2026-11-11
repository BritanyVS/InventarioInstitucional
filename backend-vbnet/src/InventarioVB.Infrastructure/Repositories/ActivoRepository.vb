Imports System.Collections.Generic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Threading.Tasks
Imports InventarioVB.Application.Interfaces
Imports InventarioVB.Domain.Entities
Imports InventarioVB.Domain.Enums
Imports InventarioVB.Domain.Interfaces

Namespace Repositories

    Public Class ActivoRepository
        Implements IActivoRepository

        Private ReadOnly _factory As ISqlConnectionFactory

        Public Sub New(factory As ISqlConnectionFactory)
            _factory = factory
        End Sub

        Public Async Function ObtenerTodosAsync() As Task(Of IEnumerable(Of Activo)) Implements IActivoRepository.ObtenerTodosAsync
            Dim lista As New List(Of Activo)()
            Using cn = _factory.Crear()
                Await cn.OpenAsync()
                Using cmd = cn.CreateCommand()
                    cmd.CommandText = "SELECT Id, Codigo, Nombre, Categoria, Marca, Modelo, NumeroSerie, Estado, Ubicacion, Valor, FechaRegistro FROM dbo.Activos ORDER BY Id"
                    Using reader = Await cmd.ExecuteReaderAsync()
                        While Await reader.ReadAsync()
                            lista.Add(Mapear(reader))
                        End While
                    End Using
                End Using
            End Using
            Return lista
        End Function

        Public Async Function ObtenerPorIdAsync(id As Integer) As Task(Of Activo) Implements IActivoRepository.ObtenerPorIdAsync
            Using cn = _factory.Crear()
                Await cn.OpenAsync()
                Using cmd = cn.CreateCommand()
                    ' Consulta parametrizada (correcta).
                    cmd.CommandText = "SELECT Id, Codigo, Nombre, Categoria, Marca, Modelo, NumeroSerie, Estado, Ubicacion, Valor, FechaRegistro FROM dbo.Activos WHERE Id = @Id"
                    cmd.Parameters.Add(New SqlParameter("@Id", id))
                    Using reader = Await cmd.ExecuteReaderAsync()
                        If Await reader.ReadAsync() Then Return Mapear(reader)
                        Return Nothing
                    End Using
                End Using
            End Using
        End Function

        ''' <summary>
        ''' ⚠️ VULNERABILIDAD INTENCIONAL (Snyk Code - SQL Injection / CWE-89):
        ''' el término de búsqueda se concatena directamente en la sentencia SQL.
        ''' Payload: ' OR '1'='1
        ''' </summary>
        Public Async Function BuscarAsync(termino As String) As Task(Of IEnumerable(Of Activo)) Implements IActivoRepository.BuscarAsync
            Dim lista As New List(Of Activo)()
            Using cn = _factory.Crear()
                Await cn.OpenAsync()
                Using cmd = cn.CreateCommand()
                    cmd.CommandText =
                        "SELECT Id, Codigo, Nombre, Categoria, Marca, Modelo, NumeroSerie, Estado, Ubicacion, Valor, FechaRegistro " &
                        "FROM dbo.Activos WHERE Codigo LIKE '%" & termino & "%' OR Nombre LIKE '%" & termino & "%' OR Categoria LIKE '%" & termino & "%'"
                    Using reader = Await cmd.ExecuteReaderAsync()
                        While Await reader.ReadAsync()
                            lista.Add(Mapear(reader))
                        End While
                    End Using
                End Using
            End Using
            Return lista
        End Function

        Public Async Function CrearAsync(a As Activo) As Task(Of Integer) Implements IActivoRepository.CrearAsync
            Using cn = _factory.Crear()
                Await cn.OpenAsync()
                Using cmd = cn.CreateCommand()
                    cmd.CommandText =
                        "INSERT INTO dbo.Activos (Codigo, Nombre, Categoria, Marca, Modelo, NumeroSerie, Estado, Ubicacion, Valor) " &
                        "VALUES (@Codigo, @Nombre, @Categoria, @Marca, @Modelo, @NumeroSerie, @Estado, @Ubicacion, @Valor); SELECT CAST(SCOPE_IDENTITY() AS INT);"
                    cmd.Parameters.Add(New SqlParameter("@Codigo", a.Codigo))
                    cmd.Parameters.Add(New SqlParameter("@Nombre", a.Nombre))
                    cmd.Parameters.Add(New SqlParameter("@Categoria", a.Categoria))
                    cmd.Parameters.Add(New SqlParameter("@Marca", If(CType(a.Marca, Object), DBNull.Value)))
                    cmd.Parameters.Add(New SqlParameter("@Modelo", If(CType(a.Modelo, Object), DBNull.Value)))
                    cmd.Parameters.Add(New SqlParameter("@NumeroSerie", If(CType(a.NumeroSerie, Object), DBNull.Value)))
                    cmd.Parameters.Add(New SqlParameter("@Estado", a.Estado.ToString()))
                    cmd.Parameters.Add(New SqlParameter("@Ubicacion", If(CType(a.Ubicacion, Object), DBNull.Value)))
                    cmd.Parameters.Add(New SqlParameter("@Valor", a.Valor))
                    Return Convert.ToInt32(Await cmd.ExecuteScalarAsync())
                End Using
            End Using
        End Function

        Public Async Function ActualizarAsync(a As Activo) As Task(Of Boolean) Implements IActivoRepository.ActualizarAsync
            Using cn = _factory.Crear()
                Await cn.OpenAsync()
                Using cmd = cn.CreateCommand()
                    cmd.CommandText =
                        "UPDATE dbo.Activos SET Codigo=@Codigo, Nombre=@Nombre, Categoria=@Categoria, Marca=@Marca, Modelo=@Modelo, " &
                        "NumeroSerie=@NumeroSerie, Ubicacion=@Ubicacion, Valor=@Valor WHERE Id=@Id"
                    cmd.Parameters.Add(New SqlParameter("@Codigo", a.Codigo))
                    cmd.Parameters.Add(New SqlParameter("@Nombre", a.Nombre))
                    cmd.Parameters.Add(New SqlParameter("@Categoria", a.Categoria))
                    cmd.Parameters.Add(New SqlParameter("@Marca", If(CType(a.Marca, Object), DBNull.Value)))
                    cmd.Parameters.Add(New SqlParameter("@Modelo", If(CType(a.Modelo, Object), DBNull.Value)))
                    cmd.Parameters.Add(New SqlParameter("@NumeroSerie", If(CType(a.NumeroSerie, Object), DBNull.Value)))
                    cmd.Parameters.Add(New SqlParameter("@Ubicacion", If(CType(a.Ubicacion, Object), DBNull.Value)))
                    cmd.Parameters.Add(New SqlParameter("@Valor", a.Valor))
                    cmd.Parameters.Add(New SqlParameter("@Id", a.Id))
                    Return Await cmd.ExecuteNonQueryAsync() > 0
                End Using
            End Using
        End Function

        Public Async Function EliminarAsync(id As Integer) As Task(Of Boolean) Implements IActivoRepository.EliminarAsync
            Using cn = _factory.Crear()
                Await cn.OpenAsync()
                Using cmd = cn.CreateCommand()
                    cmd.CommandText = "DELETE FROM dbo.Activos WHERE Id = @Id"
                    cmd.Parameters.Add(New SqlParameter("@Id", id))
                    Return Await cmd.ExecuteNonQueryAsync() > 0
                End Using
            End Using
        End Function

        Private Shared Function Mapear(r As IDataRecord) As Activo
            Dim estado As EstadoActivo
            If Not [Enum].TryParse(r.GetString(7), True, estado) Then estado = EstadoActivo.Disponible

            Return New Activo With {
                .Id = r.GetInt32(0),
                .Codigo = r.GetString(1),
                .Nombre = r.GetString(2),
                .Categoria = r.GetString(3),
                .Marca = If(r.IsDBNull(4), Nothing, r.GetString(4)),
                .Modelo = If(r.IsDBNull(5), Nothing, r.GetString(5)),
                .NumeroSerie = If(r.IsDBNull(6), Nothing, r.GetString(6)),
                .Estado = estado,
                .Ubicacion = If(r.IsDBNull(8), Nothing, r.GetString(8)),
                .Valor = r.GetDecimal(9),
                .FechaRegistro = r.GetDateTime(10)
            }
        End Function

    End Class

End Namespace
