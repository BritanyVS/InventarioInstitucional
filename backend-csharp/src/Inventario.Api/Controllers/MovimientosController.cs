using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.Api.Controllers;

[ApiController]
[Route("api/movimientos")]
public class MovimientosController : ControllerBase
{
    private readonly IMovimientoService _servicio;

    public MovimientosController(IMovimientoService servicio) => _servicio = servicio;

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos() => Ok(await _servicio.ObtenerTodosAsync());

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] CrearMovimientoDto dto)
    {
        var id = await _servicio.RegistrarAsync(dto);
        return Ok(new { id });
    }
}
