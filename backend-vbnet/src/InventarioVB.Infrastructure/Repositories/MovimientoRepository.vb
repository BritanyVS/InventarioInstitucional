Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Threading.Tasks
Imports InventarioVB.Application.Interfaces
Imports InventarioVB.Domain.Entities
Imports InventarioVB.Domain.Enums
Imports InventarioVB.Domain.Interfaces

Namespace Repositories

    Public Class MovimientoRepository
        Implements IMovimientoRepository

        Private ReadOnly _factory As ISqlConnectionFactory

        Public Sub New(factory As ISqlConnectionFactory)
            _factory = factory
        End Sub

        Public Async Function ObtenerTodosAsync() As Task(Of IEnumerable(Of Movimiento)) Implements IMovimientoRepository.ObtenerTodosAsync
            Dim lista As New List(Of Movimiento)()
            Using cn = _factory.Crear()
                Await cn.OpenAsync()
                Using cmd = cn.CreateCommand()
                    cmd.CommandText =
                        "SELECT m.Id, m.ActivoId, m.Tipo, m.Cantidad, m.Usuario, m.Observacion, m.Fecha, a.Nombre " &
                        "FROM dbo.Movimientos m INNER JOIN dbo.Activos a ON a.Id = m.ActivoId ORDER BY m.Fecha DESC"
                    Using reader = Await cmd.ExecuteReaderAsync()
                        While Await reader.ReadAsync()
                            Dim tipo As TipoMovimiento
                            If Not [Enum].TryParse(reader.GetString(2), True, tipo) Then tipo = TipoMovimiento.Entrada

                            lista.Add(New Movimiento With {
                                .Id = reader.GetInt32(0),
                                .ActivoId = reader.GetInt32(1),
                                .Tipo = tipo,
                                .Cantidad = reader.GetInt32(3),
                                .Usuario = If(reader.IsDBNull(4), Nothing, reader.GetString(4)),
                                .Observacion = If(reader.IsDBNull(5), Nothing, reader.GetString(5)),
                                .Fecha = reader.GetDateTime(6),
                                .Activo = New Activo With {.Id = reader.GetInt32(1), .Nombre = reader.GetString(7)}
                            })
                        End While
                    End Using
                End Using
            End Using
            Return lista
        End Function

        Public Async Function RegistrarAsync(m As Movimiento) As Task(Of Integer) Implements IMovimientoRepository.RegistrarAsync
            Using cn = _factory.Crear()
                Await cn.OpenAsync()
                Using cmd = cn.CreateCommand()
                    cmd.CommandText =
                        "INSERT INTO dbo.Movimientos (ActivoId, Tipo, Cantidad, Usuario, Observacion) " &
                        "VALUES (@ActivoId, @Tipo, @Cantidad, @Usuario, @Observacion); SELECT CAST(SCOPE_IDENTITY() AS INT);"
                    cmd.Parameters.Add(New SqlParameter("@ActivoId", m.ActivoId))
                    cmd.Parameters.Add(New SqlParameter("@Tipo", m.Tipo.ToString()))
                    cmd.Parameters.Add(New SqlParameter("@Cantidad", m.Cantidad))
                    cmd.Parameters.Add(New SqlParameter("@Usuario", If(CType(m.Usuario, Object), DBNull.Value)))
                    cmd.Parameters.Add(New SqlParameter("@Observacion", If(CType(m.Observacion, Object), DBNull.Value)))
                    Return Convert.ToInt32(Await cmd.ExecuteScalarAsync())
                End Using
            End Using
        End Function

    End Class

End Namespace
