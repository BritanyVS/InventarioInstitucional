Imports System.Threading.Tasks
Imports System.Web.Http
Imports InventarioVB.Application.DTOs
Imports InventarioVB.Application.Interfaces

Namespace Controllers

    <RoutePrefix("api/activos")>
    Public Class ActivosController
        Inherits ApiController

        Private ReadOnly _servicio As IActivoService

        Public Sub New()
            _servicio = CompositionRoot.CrearActivoService()
        End Sub

        <HttpGet>
        <Route("")>
        Public Async Function ObtenerTodos() As Task(Of IHttpActionResult)
            Return Ok(Await _servicio.ObtenerTodosAsync())
        End Function

        <HttpGet>
        <Route("{id:int}")>
        Public Async Function ObtenerPorId(id As Integer) As Task(Of IHttpActionResult)
            Dim activo = Await _servicio.ObtenerPorIdAsync(id)
            If activo Is Nothing Then Return NotFound()
            Return Ok(activo)
        End Function

        ''' <summary>Búsqueda de activos (el término llega por concatenación a SQL => SQLi).</summary>
        <HttpGet>
        <Route("buscar")>
        Public Async Function Buscar(<FromUri> termino As String) As Task(Of IHttpActionResult)
            Return Ok(Await _servicio.BuscarAsync(If(termino, String.Empty)))
        End Function

        <HttpPost>
        <Route("")>
        Public Async Function Crear(<FromBody> dto As CrearActivoDto) As Task(Of IHttpActionResult)
            Dim id = Await _servicio.CrearAsync(dto)
            Return Ok(New With {.id = id})
        End Function

        <HttpPut>
        <Route("{id:int}")>
        Public Async Function Actualizar(id As Integer, <FromBody> dto As CrearActivoDto) As Task(Of IHttpActionResult)
            Dim exitoso = Await _servicio.ActualizarAsync(id, dto)
            If Not exitoso Then Return NotFound()
            Return Ok(New With {.actualizado = True})
        End Function

        <HttpDelete>
        <Route("{id:int}")>
        Public Async Function Eliminar(id As Integer) As Task(Of IHttpActionResult)
            Dim exitoso = Await _servicio.EliminarAsync(id)
            If Not exitoso Then Return NotFound()
            Return Ok(New With {.eliminado = True})
        End Function

    End Class

End Namespace
