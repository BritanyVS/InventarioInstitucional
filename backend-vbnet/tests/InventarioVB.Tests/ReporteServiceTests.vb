Imports System.Threading.Tasks
Imports InventarioVB.Application.Services
Imports InventarioVB.Domain.Entities
Imports InventarioVB.Domain.Enums
Imports InventarioVB.Tests.Tests
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Tests

    <TestClass>
    Public Class ReporteServiceTests

        Private Shared Function CrearRepositorio() As ActivoRepositoryFalso
            Return New ActivoRepositoryFalso(New Activo() {
                New Activo With {.Id = 1, .Categoria = "Equipo de Cómputo", .Valor = 8500D, .Estado = EstadoActivo.Disponible},
                New Activo With {.Id = 2, .Categoria = "Impresora", .Valor = 1850D, .Estado = EstadoActivo.Asignado},
                New Activo With {.Id = 3, .Categoria = "Equipo de Cómputo", .Valor = 9000D, .Estado = EstadoActivo.Disponible}
            })
        End Function

        <TestMethod>
        Public Sub Generar_CalculaTotalesYAgrupaciones()
            Dim servicio = New ReporteService(CrearRepositorio())
            Dim reporte = servicio.GenerarAsync().GetAwaiter().GetResult()

            Assert.AreEqual(3, reporte.TotalActivos)
            Assert.AreEqual(19350D, reporte.ValorTotal)
            Assert.AreEqual(2, reporte.PorCategoria("Equipo de Cómputo"))
            Assert.AreEqual(2, reporte.PorEstado("Disponible"))
        End Sub

        <TestMethod>
        Public Sub ObtenerTodos_DevuelveLosActivos()
            Dim servicio = New ActivoService(CrearRepositorio())
            Dim activos = servicio.ObtenerTodosAsync().GetAwaiter().GetResult()

            Assert.AreEqual(3, activos.Count())
        End Sub

    End Class

End Namespace
