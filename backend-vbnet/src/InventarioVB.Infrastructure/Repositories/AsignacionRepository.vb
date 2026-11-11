Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Threading.Tasks
Imports InventarioVB.Application.Interfaces
Imports InventarioVB.Domain.Entities
Imports InventarioVB.Domain.Interfaces

Namespace Repositories

    Public Class AsignacionRepository
        Implements IAsignacionRepository

        Private ReadOnly _factory As ISqlConnectionFactory

        Public Sub New(factory As ISqlConnectionFactory)
            _factory = factory
        End Sub

        Public Async Function ObtenerTodasAsync() As Task(Of IEnumerable(Of Asignacion)) Implements IAsignacionRepository.ObtenerTodasAsync
            Dim lista As New List(Of Asignacion)()
            Using cn = _factory.Crear()
                Await cn.OpenAsync()
                Using cmd = cn.CreateCommand()
                    cmd.CommandText =
                        "SELECT s.Id, s.ActivoId, s.UsuarioAsignado, s.Area, s.FechaAsignacion, s.FechaDevolucion, a.Nombre " &
                        "FROM dbo.Asignaciones s INNER JOIN dbo.Activos a ON a.Id = s.ActivoId ORDER BY s.FechaAsignacion DESC"
                    Using reader = Await cmd.ExecuteReaderAsync()
                        While Await reader.ReadAsync()
                            lista.Add(New Asignacion With {
                                .Id = reader.GetInt32(0),
                                .ActivoId = reader.GetInt32(1),
                                .UsuarioAsignado = reader.GetString(2),
                                .Area = If(reader.IsDBNull(3), Nothing, reader.GetString(3)),
                                .FechaAsignacion = reader.GetDateTime(4),
                                .FechaDevolucion = If(reader.IsDBNull(5), CType(Nothing, DateTime?), reader.GetDateTime(5)),
                                .Activo = New Activo With {.Id = reader.GetInt32(1), .Nombre = reader.GetString(6)}
                            })
                        End While
                    End Using
                End Using
            End Using
            Return lista
        End Function

        Public Async Function CrearAsync(a As Asignacion) As Task(Of Integer) Implements IAsignacionRepository.CrearAsync
            Using cn = _factory.Crear()
                Await cn.OpenAsync()
                Using cmd = cn.CreateCommand()
                    cmd.CommandText =
                        "INSERT INTO dbo.Asignaciones (ActivoId, UsuarioAsignado, Area) VALUES (@ActivoId, @UsuarioAsignado, @Area); " &
                        "SELECT CAST(SCOPE_IDENTITY() AS INT);"
                    cmd.Parameters.Add(New SqlParameter("@ActivoId", a.ActivoId))
                    cmd.Parameters.Add(New SqlParameter("@UsuarioAsignado", a.UsuarioAsignado))
                    cmd.Parameters.Add(New SqlParameter("@Area", If(CType(a.Area, Object), DBNull.Value)))
                    Return Convert.ToInt32(Await cmd.ExecuteScalarAsync())
                End Using
            End Using
        End Function

    End Class

End Namespace
