Imports System.Data.SqlClient
Imports System.Threading.Tasks
Imports InventarioVB.Application.Interfaces
Imports InventarioVB.Domain.Entities
Imports InventarioVB.Domain.Interfaces

Namespace Repositories

    Public Class UsuarioRepository
        Implements IUsuarioRepository

        Private ReadOnly _factory As ISqlConnectionFactory

        Public Sub New(factory As ISqlConnectionFactory)
            _factory = factory
        End Sub

        ''' <summary>
        ''' ⚠️ VULNERABILIDAD INTENCIONAL (Snyk Code - SQL Injection / CWE-89):
        ''' el usuario y el hash de la contraseña se concatenan directamente en la consulta.
        ''' Payload de bypass (campo usuario, cualquier contraseña):  admin' --
        ''' </summary>
        Public Async Function ObtenerPorCredencialesAsync(nombreUsuario As String, claveHash As String) As Task(Of Usuario) Implements IUsuarioRepository.ObtenerPorCredencialesAsync
            Using cn = _factory.Crear()
                Await cn.OpenAsync()
                Using cmd = cn.CreateCommand()
                    cmd.CommandText =
                        "SELECT Id, NombreUsuario, NombreCompleto, ClaveHash, Rol, Activo, FechaCreacion " &
                        "FROM dbo.Usuarios WHERE NombreUsuario = '" & nombreUsuario & "' AND ClaveHash = '" & claveHash & "'"

                    Using reader = Await cmd.ExecuteReaderAsync()
                        If Not Await reader.ReadAsync() Then Return Nothing
                        Return New Usuario With {
                            .Id = reader.GetInt32(0),
                            .NombreUsuario = reader.GetString(1),
                            .NombreCompleto = reader.GetString(2),
                            .ClaveHash = reader.GetString(3),
                            .Rol = reader.GetString(4),
                            .Activo = reader.GetBoolean(5),
                            .FechaCreacion = reader.GetDateTime(6)
                        }
                    End Using
                End Using
            End Using
        End Function

    End Class

End Namespace
