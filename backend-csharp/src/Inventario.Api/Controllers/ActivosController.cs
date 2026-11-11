using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.Api.Controllers;

[ApiController]
[Route("api/activos")]
public class ActivosController : ControllerBase
{
    private readonly IActivoService _servicio;

    public ActivosController(IActivoService servicio) => _servicio = servicio;

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos() => Ok(await _servicio.ObtenerTodosAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var activo = await _servicio.ObtenerPorIdAsync(id);
        return activo is null ? NotFound(new { mensaje = "Activo no encontrado." }) : Ok(activo);
    }

    /// <summary>Búsqueda de activos. El parámetro "termino" llega a la capa de datos por concatenación (SQLi).</summary>
    [HttpGet("buscar")]
    public async Task<IActionResult> Buscar([FromQuery] string termino)
        => Ok(await _servicio.BuscarAsync(termino ?? string.Empty));

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearActivoDto dto)
    {
        var id = await _servicio.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] CrearActivoDto dto)
        => await _servicio.ActualizarAsync(id, dto)
            ? NoContent()
            : NotFound(new { mensaje = "Activo no encontrado." });

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
        => await _servicio.EliminarAsync(id)
            ? NoContent()
            : NotFound(new { mensaje = "Activo no encontrado." });
}
