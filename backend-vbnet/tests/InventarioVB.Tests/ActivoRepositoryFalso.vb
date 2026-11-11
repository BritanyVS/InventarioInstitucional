Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports InventarioVB.Domain.Entities
Imports InventarioVB.Domain.Interfaces

Namespace Tests

    ''' <summary>Repositorio en memoria para pruebas unitarias (sin base de datos).</summary>
    Friend Class ActivoRepositoryFalso
        Implements IActivoRepository

        Private ReadOnly _datos As List(Of Activo)

        Public Sub New(Optional datos As IEnumerable(Of Activo) = Nothing)
            _datos = If(datos IsNot Nothing, datos.ToList(), New List(Of Activo)())
        End Sub

        Public Function ObtenerTodosAsync() As Task(Of IEnumerable(Of Activo)) Implements IActivoRepository.ObtenerTodosAsync
            Return Task.FromResult(Of IEnumerable(Of Activo))(_datos)
        End Function

        Public Function ObtenerPorIdAsync(id As Integer) As Task(Of Activo) Implements IActivoRepository.ObtenerPorIdAsync
            Return Task.FromResult(_datos.FirstOrDefault(Function(a) a.Id = id))
        End Function

        Public Function BuscarAsync(termino As String) As Task(Of IEnumerable(Of Activo)) Implements IActivoRepository.BuscarAsync
            Return Task.FromResult(Of IEnumerable(Of Activo))(
                _datos.Where(Function(a) a.Nombre.IndexOf(termino, System.StringComparison.OrdinalIgnoreCase) >= 0))
        End Function

        Public Function CrearAsync(activo As Activo) As Task(Of Integer) Implements IActivoRepository.CrearAsync
            activo.Id = _datos.Count + 1
            _datos.Add(activo)
            Return Task.FromResult(activo.Id)
        End Function

        Public Function ActualizarAsync(activo As Activo) As Task(Of Boolean) Implements IActivoRepository.ActualizarAsync
            Return Task.FromResult(True)
        End Function

        Public Function EliminarAsync(id As Integer) As Task(Of Boolean) Implements IActivoRepository.EliminarAsync
            Return Task.FromResult(_datos.RemoveAll(Function(a) a.Id = id) > 0)
        End Function

    End Class

End Namespace
