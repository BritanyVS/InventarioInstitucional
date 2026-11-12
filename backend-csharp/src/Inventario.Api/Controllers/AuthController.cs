using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.Api.Controllers;

[ApiController]
[Route("api/auth")] 
public class AuthController : ControllerBase
{
    private readonly IAuthService _servicio;

    public AuthController(IAuthService servicio) => _servicio = servicio;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var resultado = await _servicio.LoginAsync(dto);
        return resultado.Exitoso ? Ok(resultado) : Unauthorized(resultado);
    }
}
