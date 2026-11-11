Imports System
Imports Microsoft.Owin.Hosting

''' <summary>
''' Punto de entrada del backend VB.NET (.NET Framework 4.8).
''' Levanta ASP.NET Web API 2 mediante OWIN self-host (HttpListener).
''' </summary>
Module Program

    Sub Main()
        Dim baseAddress As String = "http://localhost:5081/"

        Console.WriteLine("========================================================")
        Console.WriteLine(" Inventario Institucional - Backend VB.NET / .NET 4.8")
        Console.WriteLine("========================================================")

        Try
            Using app = WebApp.Start(Of Startup)(baseAddress)
                Console.WriteLine($"API escuchando en {baseAddress}")
                Console.WriteLine($"Prueba: {baseAddress}api/activos")
                Console.WriteLine("Presione ENTER para detener el servidor...")

                If Console.IsInputRedirected Then
                    ' Modo sin consola interactiva (servicios, pruebas): permanecer activo.
                    System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite)
                Else
                    Console.ReadLine()
                End If
            End Using
        Catch ex As Exception
            Console.WriteLine("No se pudo iniciar el servidor: " & ex.Message)
            Console.WriteLine("Si el error es 'Acceso denegado', ejecute una vez como administrador:")
            Console.WriteLine("  netsh http add urlacl url=http://localhost:5081/ user=Everyone")
            Console.ReadLine()
        End Try
    End Sub

End Module
