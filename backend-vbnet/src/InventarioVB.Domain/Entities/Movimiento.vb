Imports InventarioVB.Domain.Enums

Namespace Entities

    ''' <summary>Movimiento de entrada o salida de un activo.</summary>
    Public Class Movimiento
        Public Property Id As Integer
        Public Property ActivoId As Integer
        Public Property Tipo As TipoMovimiento
        Public Property Cantidad As Integer = 1
        Public Property Usuario As String
        Public Property Observacion As String
        Public Property Fecha As DateTime = DateTime.Now
        Public Property Activo As Activo
    End Class

End Namespace
