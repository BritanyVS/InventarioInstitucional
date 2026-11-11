Imports System.IO
Imports InventarioVB.Application.Interfaces

Namespace Services

    ''' <summary>
    ''' Servicio de lectura de reportes exportados a disco.
    '''
    ''' ⚠️ VULNERABILIDAD INTENCIONAL (Snyk Code - Path Traversal / CWE-22):
    ''' el nombre del archivo recibido desde el cliente se concatena a la carpeta base
    ''' sin validación, permitiendo escapar del directorio.
    ''' Payload de ejemplo: ..\..\..\Windows\win.ini
    ''' </summary>
    Public Class FileService
        Implements IFileService

        Private ReadOnly _carpetaBase As String =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes")

        Public Function LeerReporte(nombreArchivo As String) As String Implements IFileService.LeerReporte
            If Not Directory.Exists(_carpetaBase) Then Directory.CreateDirectory(_carpetaBase)

            ' Concatenación directa => permite "..\..\" para leer archivos arbitrarios.
            Dim rutaCompleta As String = _carpetaBase & "\" & nombreArchivo

            If Not File.Exists(rutaCompleta) Then
                Return "No existe el reporte solicitado: " & nombreArchivo
            End If

            Return File.ReadAllText(rutaCompleta)
        End Function

    End Class

End Namespace
