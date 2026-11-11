Namespace Entities

    ''' <summary>Asignación de un activo a un empleado o área.</summary>
    Public Class Asignacion
        Public Property Id As Integer
        Public Property ActivoId As Integer
        Public Property UsuarioAsignado As String = String.Empty
        Public Property Area As String
        Public Property FechaAsignacion As DateTime = DateTime.Now
        Public Property FechaDevolucion As DateTime?
        Public Property Activo As Activo
    End Class

End Namespace
