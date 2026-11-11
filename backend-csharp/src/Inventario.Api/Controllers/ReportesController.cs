using Inventario.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.Api.Controllers;

[ApiController]
[Route("api/reportes")]
public class ReportesController : ControllerBase
{
    private readonly IReporteService _servicio;

    public ReportesController(IReporteService servicio) => _servicio = servicio;

    [HttpGet("inventario")]
    public async Task<IActionResult> Inventario() => Ok(await _servicio.GenerarAsync());
}
