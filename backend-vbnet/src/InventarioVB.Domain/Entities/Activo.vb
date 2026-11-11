Imports InventarioVB.Domain.Enums

Namespace Entities

    ''' <summary>Activo institucional (equipo, mobiliario, etc.).</summary>
    Public Class Activo
        Public Property Id As Integer
        Public Property Codigo As String = String.Empty
        Public Property Nombre As String = String.Empty
        Public Property Categoria As String = String.Empty
        Public Property Marca As String
        Public Property Modelo As String
        Public Property NumeroSerie As String
        Public Property Estado As EstadoActivo = EstadoActivo.Disponible
        Public Property Ubicacion As String
        Public Property Valor As Decimal
        Public Property FechaRegistro As DateTime = DateTime.Now
    End Class

End Namespace
