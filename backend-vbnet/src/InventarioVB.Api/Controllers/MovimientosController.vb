Imports System.Threading.Tasks
Imports System.Web.Http
Imports InventarioVB.Application.DTOs
Imports InventarioVB.Application.Interfaces

Namespace Controllers

    <RoutePrefix("api/movimientos")>
    Public Class MovimientosController
        Inherits ApiController

        Private ReadOnly _servicio As IMovimientoService

        Public Sub New()
            _servicio = CompositionRoot.CrearMovimientoService()
        End Sub

        <HttpGet>
        <Route("")>
        Public Async Function ObtenerTodos() As Task(Of IHttpActionResult)
            Return Ok(Await _servicio.ObtenerTodosAsync())
        End Function

        <HttpPost>
        <Route("")>
        Public Async Function Registrar(<FromBody> dto As CrearMovimientoDto) As Task(Of IHttpActionResult)
            Dim id = Await _servicio.RegistrarAsync(dto)
            Return Ok(New With {.id = id})
        End Function

    End Class

    <RoutePrefix("api/asignaciones")>
    Public Class AsignacionesController
        Inherits ApiController

        Private ReadOnly _servicio As IAsignacionService

        Public Sub New()
            _servicio = CompositionRoot.CrearAsignacionService()
        End Sub

        <HttpGet>
        <Route("")>
        Public Async Function ObtenerTodas() As Task(Of IHttpActionResult)
            Return Ok(Await _servicio.ObtenerTodasAsync())
        End Function

        <HttpPost>
        <Route("")>
        Public Async Function Crear(<FromBody> dto As CrearAsignacionDto) As Task(Of IHttpActionResult)
            Dim id = Await _servicio.CrearAsync(dto)
            Return Ok(New With {.id = id})
        End Function

    End Class

    <RoutePrefix("api/auth")>
    Public Class AuthController
        Inherits ApiController

        Private ReadOnly _servicio As IAuthService

        Public Sub New()
            _servicio = CompositionRoot.CrearAuthService()
        End Sub

        <HttpPost>
        <Route("login")>
        Public Async Function Login(<FromBody> dto As LoginDto) As Task(Of IHttpActionResult)
            Dim resultado = Await _servicio.LoginAsync(dto)
            If Not resultado.Exitoso Then Return Content(Net.HttpStatusCode.Unauthorized, resultado)
            Return Ok(resultado)
        End Function

    End Class

End Namespace
