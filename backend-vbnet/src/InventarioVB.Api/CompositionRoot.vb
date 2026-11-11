Imports System.Configuration
Imports InventarioVB.Application.Interfaces
Imports InventarioVB.Application.Services
Imports InventarioVB.Infrastructure.Data
Imports InventarioVB.Infrastructure.Repositories
Imports InventarioVB.Infrastructure.Security
Imports InventarioVB.Infrastructure.Services

''' <summary>
''' Composition Root: punto único donde se construyen e inyectan las dependencias.
''' Mantiene la Api desacoplada de las implementaciones concretas.
''' </summary>
Public Module CompositionRoot

    Private ReadOnly _factory As ISqlConnectionFactory

    Sub New()
        Dim cs = ConfigurationManager.ConnectionStrings("InventarioDb")
        _factory = New SqlConnectionFactory(If(cs IsNot Nothing, cs.ConnectionString, Nothing))
    End Sub

    Public Function CrearActivoService() As IActivoService
        Return New ActivoService(New ActivoRepository(_factory))
    End Function

    Public Function CrearMovimientoService() As IMovimientoService
        Return New MovimientoService(New MovimientoRepository(_factory))
    End Function

    Public Function CrearAsignacionService() As IAsignacionService
        Return New AsignacionService(New AsignacionRepository(_factory))
    End Function

    Public Function CrearAuthService() As IAuthService
        Return New AuthService(New UsuarioRepository(_factory), New Md5PasswordHasher())
    End Function

    Public Function CrearReporteService() As IReporteService
        Return New ReporteService(New ActivoRepository(_factory))
    End Function

    Public Function CrearFileService() As IFileService
        Return New FileService()
    End Function

End Module
