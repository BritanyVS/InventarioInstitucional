Imports InventarioVB.Domain.Enums

Namespace DTOs

    Public Class ActivoDto
        Public Property Id As Integer
        Public Property Codigo As String = String.Empty
        Public Property Nombre As String = String.Empty
        Public Property Categoria As String = String.Empty
        Public Property Marca As String
        Public Property Modelo As String
        Public Property NumeroSerie As String
        Public Property Estado As EstadoActivo
        Public Property Ubicacion As String
        Public Property Valor As Decimal
        Public Property FechaRegistro As DateTime
    End Class

    Public Class CrearActivoDto
        Public Property Codigo As String = String.Empty
        Public Property Nombre As String = String.Empty
        Public Property Categoria As String = String.Empty
        Public Property Marca As String
        Public Property Modelo As String
        Public Property NumeroSerie As String
        Public Property Ubicacion As String
        Public Property Valor As Decimal
    End Class

    Public Class MovimientoDto
        Public Property Id As Integer
        Public Property ActivoId As Integer
        Public Property ActivoNombre As String
        Public Property Tipo As TipoMovimiento
        Public Property Cantidad As Integer
        Public Property Usuario As String
        Public Property Observacion As String
        Public Property Fecha As DateTime
    End Class

    Public Class CrearMovimientoDto
        Public Property ActivoId As Integer
        Public Property Tipo As TipoMovimiento
        Public Property Cantidad As Integer = 1
        Public Property Usuario As String
        Public Property Observacion As String
    End Class

    Public Class AsignacionDto
        Public Property Id As Integer
        Public Property ActivoId As Integer
        Public Property ActivoNombre As String
        Public Property UsuarioAsignado As String = String.Empty
        Public Property Area As String
        Public Property FechaAsignacion As DateTime
        Public Property FechaDevolucion As DateTime?
    End Class

    Public Class CrearAsignacionDto
        Public Property ActivoId As Integer
        Public Property UsuarioAsignado As String = String.Empty
        Public Property Area As String
    End Class

    Public Class LoginDto
        Public Property Usuario As String = String.Empty
        Public Property Clave As String = String.Empty
    End Class

    Public Class LoginResultDto
        Public Property Exitoso As Boolean
        Public Property Token As String
        Public Property NombreCompleto As String
        Public Property Rol As String
        Public Property Mensaje As String
    End Class

    Public Class ReporteInventarioDto
        Public Property TotalActivos As Integer
        Public Property ValorTotal As Decimal
        Public Property PorCategoria As New Dictionary(Of String, Integer)()
        Public Property PorEstado As New Dictionary(Of String, Integer)()
    End Class

End Namespace
