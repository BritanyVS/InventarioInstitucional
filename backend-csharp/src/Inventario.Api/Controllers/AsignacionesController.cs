using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.Api.Controllers;

[ApiController]
[Route("api/asignaciones")]
public class AsignacionesController : ControllerBase
{
    private readonly IAsignacionService _servicio;

    public AsignacionesController(IAsignacionService servicio) => _servicio = servicio;

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas() => Ok(await _servicio.ObtenerTodasAsync());

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearAsignacionDto dto)
    {
        var id = await _servicio.CrearAsync(dto);
        return Ok(new { id });
    }
}
