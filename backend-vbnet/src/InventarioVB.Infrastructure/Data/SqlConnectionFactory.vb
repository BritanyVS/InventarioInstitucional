Imports System.Data.Common
Imports System.Data.SqlClient
Imports InventarioVB.Application.Interfaces

Namespace Data

    ''' <summary>
    ''' Fábrica de conexiones a SQL Server.
    '''
    ''' ⚠️ VULNERABILIDAD INTENCIONAL (Snyk Code - Hardcoded Credentials / CWE-798):
    ''' credenciales de base de datos embebidas en el código fuente.
    ''' </summary>
    Public Class SqlConnectionFactory
        Implements ISqlConnectionFactory

        ' Credenciales en duro usadas como respaldo cuando no hay configuración.
        Private Const CadenaPorDefecto As String =
            "Server=.\SQLEXPRESS;Database=InventarioInstitucional;User Id=sa;Password=Sup3rS3cret!;TrustServerCertificate=True;"

        Private ReadOnly _cadena As String

        Public ReadOnly Property CadenaConexion As String Implements ISqlConnectionFactory.CadenaConexion
            Get
                Return _cadena
            End Get
        End Property

        Public Sub New(cadenaConexion As String)
            _cadena = If(String.IsNullOrWhiteSpace(cadenaConexion), CadenaPorDefecto, cadenaConexion)
        End Sub

        Public Function Crear() As DbConnection Implements ISqlConnectionFactory.Crear
            Return New SqlConnection(_cadena)
        End Function

    End Class

End Namespace
