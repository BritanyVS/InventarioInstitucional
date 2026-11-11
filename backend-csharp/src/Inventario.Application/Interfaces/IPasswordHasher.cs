namespace Inventario.Application.Interfaces;

/// <summary>Servicio de hash de contraseñas (implementado en Infrastructure).</summary>
public interface IPasswordHasher
{
    string Hash(string clavePlana);
    bool Verificar(string clavePlana, string hashAlmacenado);
}
