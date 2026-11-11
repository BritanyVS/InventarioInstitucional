Imports System.Threading.Tasks
Imports System.Web.Http
Imports InventarioVB.Application.Interfaces

Namespace Controllers

    <RoutePrefix("api/reportes")>
    Public Class ReportesController
        Inherits ApiController

        Private ReadOnly _servicio As IReporteService
        Private ReadOnly _archivos As IFileService

        Public Sub New()
            _servicio = CompositionRoot.CrearReporteService()
            _archivos = CompositionRoot.CrearFileService()
        End Sub

        <HttpGet>
        <Route("inventario")>
        Public Async Function Inventario() As Task(Of IHttpActionResult)
            Return Ok(Await _servicio.GenerarAsync())
        End Function

        ''' <summary>
        ''' ⚠️ VULNERABILIDAD INTENCIONAL (Path Traversal / CWE-22):
        ''' el parámetro "nombre" se usa para leer un archivo sin validar la ruta.
        ''' Prueba: /api/reportes/archivo?nombre=..\..\..\Windows\win.ini
        ''' </summary>
        <HttpGet>
        <Route("archivo")>
        Public Function Archivo(<FromUri> nombre As String) As IHttpActionResult
            Dim contenido = _archivos.LeerReporte(nombre)
            Return Ok(New With {.archivo = nombre, .contenido = contenido})
        End Function

    End Class

End Namespace
