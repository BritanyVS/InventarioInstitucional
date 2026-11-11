Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports InventarioVB.Application.DTOs
Imports InventarioVB.Application.Interfaces
Imports InventarioVB.Domain.Entities
Imports InventarioVB.Domain.Interfaces

Namespace Services

    Public Class ActivoService
        Implements IActivoService

        Private ReadOnly _repositorio As IActivoRepository

        Public Sub New(repositorio As IActivoRepository)
            _repositorio = repositorio
        End Sub

        Public Async Function ObtenerTodosAsync() As Task(Of IEnumerable(Of ActivoDto)) Implements IActivoService.ObtenerTodosAsync
            Dim activos = Await _repositorio.ObtenerTodosAsync()
            Return activos.Select(AddressOf Mapear)
        End Function

        Public Async Function ObtenerPorIdAsync(id As Integer) As Task(Of ActivoDto) Implements IActivoService.ObtenerPorIdAsync
            Dim activo = Await _repositorio.ObtenerPorIdAsync(id)
            If activo Is Nothing Then Return Nothing
            Return Mapear(activo)
        End Function

        Public Async Function BuscarAsync(termino As String) As Task(Of IEnumerable(Of ActivoDto)) Implements IActivoService.BuscarAsync
            Dim activos = Await _repositorio.BuscarAsync(termino)
            Return activos.Select(AddressOf Mapear)
        End Function

        Public Function CrearAsync(dto As CrearActivoDto) As Task(Of Integer) Implements IActivoService.CrearAsync
            Return _repositorio.CrearAsync(New Activo With {
                .Codigo = dto.Codigo,
                .Nombre = dto.Nombre,
                .Categoria = dto.Categoria,
                .Marca = dto.Marca,
                .Modelo = dto.Modelo,
                .NumeroSerie = dto.NumeroSerie,
                .Ubicacion = dto.Ubicacion,
                .Valor = dto.Valor
            })
        End Function

        Public Async Function ActualizarAsync(id As Integer, dto As CrearActivoDto) As Task(Of Boolean) Implements IActivoService.ActualizarAsync
            Dim existente = Await _repositorio.ObtenerPorIdAsync(id)
            If existente Is Nothing Then Return False

            existente.Codigo = dto.Codigo
            existente.Nombre = dto.Nombre
            existente.Categoria = dto.Categoria
            existente.Marca = dto.Marca
            existente.Modelo = dto.Modelo
            existente.NumeroSerie = dto.NumeroSerie
            existente.Ubicacion = dto.Ubicacion
            existente.Valor = dto.Valor
            Return Await _repositorio.ActualizarAsync(existente)
        End Function

        Public Function EliminarAsync(id As Integer) As Task(Of Boolean) Implements IActivoService.EliminarAsync
            Return _repositorio.EliminarAsync(id)
        End Function

        Private Shared Function Mapear(a As Activo) As ActivoDto
            Return New ActivoDto With {
                .Id = a.Id,
                .Codigo = a.Codigo,
                .Nombre = a.Nombre,
                .Categoria = a.Categoria,
                .Marca = a.Marca,
                .Modelo = a.Modelo,
                .NumeroSerie = a.NumeroSerie,
                .Estado = a.Estado,
                .Ubicacion = a.Ubicacion,
                .Valor = a.Valor,
                .FechaRegistro = a.FechaRegistro
            }
        End Function

    End Class

    Public Class MovimientoService
        Implements IMovimientoService

        Private ReadOnly _repositorio As IMovimientoRepository

        Public Sub New(repositorio As IMovimientoRepository)
            _repositorio = repositorio
        End Sub

        Public Async Function ObtenerTodosAsync() As Task(Of IEnumerable(Of MovimientoDto)) Implements IMovimientoService.ObtenerTodosAsync
            Dim movimientos = Await _repositorio.ObtenerTodosAsync()
            Return movimientos.Select(Function(m) New MovimientoDto With {
                .Id = m.Id,
                .ActivoId = m.ActivoId,
                .ActivoNombre = If(m.Activo IsNot Nothing, m.Activo.Nombre, Nothing),
                .Tipo = m.Tipo,
                .Cantidad = m.Cantidad,
                .Usuario = m.Usuario,
                .Observacion = m.Observacion,
                .Fecha = m.Fecha
            })
        End Function

        Public Function RegistrarAsync(dto As CrearMovimientoDto) As Task(Of Integer) Implements IMovimientoService.RegistrarAsync
            Return _repositorio.RegistrarAsync(New Movimiento With {
                .ActivoId = dto.ActivoId,
                .Tipo = dto.Tipo,
                .Cantidad = dto.Cantidad,
                .Usuario = dto.Usuario,
                .Observacion = dto.Observacion
            })
        End Function

    End Class

    Public Class AsignacionService
        Implements IAsignacionService

        Private ReadOnly _repositorio As IAsignacionRepository

        Public Sub New(repositorio As IAsignacionRepository)
            _repositorio = repositorio
        End Sub

        Public Async Function ObtenerTodasAsync() As Task(Of IEnumerable(Of AsignacionDto)) Implements IAsignacionService.ObtenerTodasAsync
            Dim asignaciones = Await _repositorio.ObtenerTodasAsync()
            Return asignaciones.Select(Function(a) New AsignacionDto With {
                .Id = a.Id,
                .ActivoId = a.ActivoId,
                .ActivoNombre = If(a.Activo IsNot Nothing, a.Activo.Nombre, Nothing),
                .UsuarioAsignado = a.UsuarioAsignado,
                .Area = a.Area,
                .FechaAsignacion = a.FechaAsignacion,
                .FechaDevolucion = a.FechaDevolucion
            })
        End Function

        Public Function CrearAsync(dto As CrearAsignacionDto) As Task(Of Integer) Implements IAsignacionService.CrearAsync
            Return _repositorio.CrearAsync(New Asignacion With {
                .ActivoId = dto.ActivoId,
                .UsuarioAsignado = dto.UsuarioAsignado,
                .Area = dto.Area
            })
        End Function

    End Class

    Public Class AuthService
        Implements IAuthService

        Private ReadOnly _usuarios As IUsuarioRepository
        Private ReadOnly _hasher As IPasswordHasher

        Public Sub New(usuarios As IUsuarioRepository, hasher As IPasswordHasher)
            _usuarios = usuarios
            _hasher = hasher
        End Sub

        Public Async Function LoginAsync(dto As LoginDto) As Task(Of LoginResultDto) Implements IAuthService.LoginAsync
            Dim hash = _hasher.Hash(dto.Clave)
            Dim usuario = Await _usuarios.ObtenerPorCredencialesAsync(dto.Usuario, hash)

            If usuario Is Nothing OrElse Not usuario.Activo Then
                Return New LoginResultDto With {.Exitoso = False, .Mensaje = "Usuario o contraseña incorrectos."}
            End If

            Dim token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{usuario.NombreUsuario}:{usuario.Rol}"))

            Return New LoginResultDto With {
                .Exitoso = True,
                .Token = token,
                .NombreCompleto = usuario.NombreCompleto,
                .Rol = usuario.Rol
            }
        End Function

    End Class

    Public Class ReporteService
        Implements IReporteService

        Private ReadOnly _activos As IActivoRepository

        Public Sub New(activos As IActivoRepository)
            _activos = activos
        End Sub

        Public Async Function GenerarAsync() As Task(Of ReporteInventarioDto) Implements IReporteService.GenerarAsync
            Dim lista = (Await _activos.ObtenerTodosAsync()).ToList()

            Return New ReporteInventarioDto With {
                .TotalActivos = lista.Count,
                .ValorTotal = lista.Sum(Function(a) a.Valor),
                .PorCategoria = lista.GroupBy(Function(a) a.Categoria).ToDictionary(Function(g) g.Key, Function(g) g.Count()),
                .PorEstado = lista.GroupBy(Function(a) a.Estado.ToString()).ToDictionary(Function(g) g.Key, Function(g) g.Count())
            }
        End Function

    End Class

End Namespace
