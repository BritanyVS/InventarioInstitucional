Imports System.Security.Cryptography
Imports System.Text
Imports InventarioVB.Application.Interfaces

Namespace Security

    ''' <summary>
    ''' Servicio de hash de contraseñas.
    '''
    ''' ⚠️ VULNERABILIDAD INTENCIONAL (Snyk Code - Weak Cryptography / CWE-327):
    ''' se usa MD5 (algoritmo roto) sin salt para almacenar contraseñas.
    ''' </summary>
    Public Class Md5PasswordHasher
        Implements IPasswordHasher

        Public Function Hash(clavePlana As String) As String Implements IPasswordHasher.Hash
            Using algoritmo = MD5.Create()
                Dim bytes = Encoding.UTF8.GetBytes(clavePlana)
                Dim resultado = algoritmo.ComputeHash(bytes)
                Return BitConverter.ToString(resultado).Replace("-", "").ToLowerInvariant()
            End Using
        End Function

        Public Function Verificar(clavePlana As String, hashAlmacenado As String) As Boolean Implements IPasswordHasher.Verificar
            Return String.Equals(Hash(clavePlana), hashAlmacenado, StringComparison.OrdinalIgnoreCase)
        End Function

    End Class

End Namespace
