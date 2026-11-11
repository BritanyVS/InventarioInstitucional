Imports System.Collections.Generic
Imports System.Data.Common
Imports System.Threading.Tasks
Imports InventarioVB.Application.DTOs

Namespace Interfaces

    ''' <summary>Fábrica de conexiones a SQL Server (implementada en Infrastructure).</summary>
    Public Interface ISqlConnectionFactory
        Function Crear() As DbConnection
        ReadOnly Property CadenaConexion As String
    End Interface

    ''' <summary>Servicio de hash de contraseñas.</summary>
    Public Interface IPasswordHasher
        Function Hash(clavePlana As String) As String
        Function Verificar(clavePlana As String, hashAlmacenado As String) As Boolean
    End Interface

    ''' <summary>Servicio de archivos (exportación de reportes).</summary>
    Public Interface IFileService
        Function LeerReporte(nombreArchivo As String) As String
    End Interface

    Public Interface IActivoService
        Function ObtenerTodosAsync() As Task(Of IEnumerable(Of ActivoDto))
        Function ObtenerPorIdAsync(id As Integer) As Task(Of ActivoDto)
        Function BuscarAsync(termino As String) As Task(Of IEnumerable(Of ActivoDto))
        Function CrearAsync(dto As CrearActivoDto) As Task(Of Integer)
        Function ActualizarAsync(id As Integer, dto As CrearActivoDto) As Task(Of Boolean)
        Function EliminarAsync(id As Integer) As Task(Of Boolean)
    End Interface

    Public Interface IMovimientoService
        Function ObtenerTodosAsync() As Task(Of IEnumerable(Of MovimientoDto))
        Function RegistrarAsync(dto As CrearMovimientoDto) As Task(Of Integer)
    End Interface

    Public Interface IAsignacionService
        Function ObtenerTodasAsync() As Task(Of IEnumerable(Of AsignacionDto))
        Function CrearAsync(dto As CrearAsignacionDto) As Task(Of Integer)
    End Interface

    Public Interface IAuthService
        Function LoginAsync(dto As LoginDto) As Task(Of LoginResultDto)
    End Interface

    Public Interface IReporteService
        Function GenerarAsync() As Task(Of ReporteInventarioDto)
    End Interface

End Namespace
