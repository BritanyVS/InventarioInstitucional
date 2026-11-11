using Inventario.Application.Interfaces;

namespace Inventario.Infrastructure.Security;

/// <summary>
/// Servicio de hash de contraseñas.
///
/// ⚠️ VULNERABILIDAD INTENCIONAL (Snyk Code - Weak Cryptography / CWE-327 y CWE-916):
/// se utiliza MD5, un algoritmo criptográficamente roto, sin "salt" y con una sola
/// iteración, para almacenar contraseñas. Snyk Code lo reporta como
/// "Use of a broken or risky cryptographic algorithm" / "Insufficient password hash".
///
/// Alternativa segura: BCrypt / PBKDF2 / Argon2 con salt aleatorio.
/// </summary>
public class Md5PasswordHasher : IPasswordHasher
{
    public string Hash(string clavePlana)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(clavePlana);
        var hash = md5.ComputeHash(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public bool Verificar(string clavePlana, string hashAlmacenado)
        => string.Equals(Hash(clavePlana), hashAlmacenado, StringComparison.OrdinalIgnoreCase);
}
