Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports InventarioVB.Domain.Entities

Namespace Interfaces

    Public Interface IActivoRepository
        Function ObtenerTodosAsync() As Task(Of IEnumerable(Of Activo))
        Function ObtenerPorIdAsync(id As Integer) As Task(Of Activo)
        Function BuscarAsync(termino As String) As Task(Of IEnumerable(Of Activo))
        Function CrearAsync(activo As Activo) As Task(Of Integer)
        Function ActualizarAsync(activo As Activo) As Task(Of Boolean)
        Function EliminarAsync(id As Integer) As Task(Of Boolean)
    End Interface

    Public Interface IMovimientoRepository
        Function ObtenerTodosAsync() As Task(Of IEnumerable(Of Movimiento))
        Function RegistrarAsync(movimiento As Movimiento) As Task(Of Integer)
    End Interface

    Public Interface IAsignacionRepository
        Function ObtenerTodasAsync() As Task(Of IEnumerable(Of Asignacion))
        Function CrearAsync(asignacion As Asignacion) As Task(Of Integer)
    End Interface

    Public Interface IUsuarioRepository
        Function ObtenerPorCredencialesAsync(nombreUsuario As String, claveHash As String) As Task(Of Usuario)
    End Interface

End Namespace
