using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using Inventario.Domain.Interfaces;

namespace Inventario.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IPasswordHasher _hasher;

    public AuthService(IUsuarioRepository usuarios, IPasswordHasher hasher)
    {
        _usuarios = usuarios;
        _hasher = hasher;
    }

    public async Task<LoginResultDto> LoginAsync(LoginDto dto)
    {
        var hash = _hasher.Hash(dto.Clave);
        var usuario = await _usuarios.ObtenerPorCredencialesAsync(dto.Usuario, hash);

        if (usuario is null || !usuario.Activo)
            return new LoginResultDto { Exitoso = false, Mensaje = "Usuario o contraseña incorrectos." };

        // Token simple de demostración (NO es un JWT firmado real).
        var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{usuario.NombreUsuario}:{usuario.Rol}"));

        return new LoginResultDto
        {
            Exitoso = true,
            Token = token,
            NombreCompleto = usuario.NombreCompleto,
            Rol = usuario.Rol
        };
    }
}
