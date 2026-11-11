Namespace Entities

    ''' <summary>Usuario del sistema.</summary>
    Public Class Usuario
        Public Property Id As Integer
        Public Property NombreUsuario As String = String.Empty
        Public Property NombreCompleto As String = String.Empty
        Public Property ClaveHash As String = String.Empty
        Public Property Rol As String = "Consulta"
        Public Property Activo As Boolean = True
        Public Property FechaCreacion As DateTime = DateTime.Now
    End Class

End Namespace
