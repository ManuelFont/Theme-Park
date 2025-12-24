using Dtos;
using Microsoft.AspNetCore.Mvc;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Filtros;

namespace ParqueTematico.WebApi.Controllers;

[ApiController]
[Route("api/accesos")]
public class AccesoAtraccionController(
    IComandoAccesoAtraccion comandoService,
    IConsultaAccesoAtraccion consultaService,
    IAuthService authService) : ControllerBase
{
    private readonly IComandoAccesoAtraccion _comandoService = comandoService;
    private readonly IConsultaAccesoAtraccion _consultaService = consultaService;
    private readonly IAuthService _authService = authService;

    [RequiereRol("Operador")]
    [HttpPost("ingreso")]
    public IActionResult RegistrarIngreso([FromBody] RegistrarIngresoRequest request)
    {
        var accesoId = _comandoService.RegistrarIngreso(request.TicketId, request.AtraccionId);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = accesoId }, new { accesoId });
    }

    [RequiereRol("Operador")]
    [HttpPut("egreso/{accesoId}")]
    public IActionResult RegistrarEgreso(Guid accesoId)
    {
        _comandoService.RegistrarEgreso(accesoId);
        return NoContent();
    }

    [RequiereRol("Operador")]
    [HttpGet("aforo/{atraccionId}")]
    public IActionResult ObtenerAforo(Guid atraccionId)
    {
        var aforoActual = _consultaService.ObtenerAforoActual(atraccionId);
        return Ok(new { aforoActual });
    }

    [RequiereRol("Operador")]
    [HttpGet("{id}")]
    public ActionResult<AccesoAtraccionDto> ObtenerPorId(Guid id)
    {
        var acceso = _consultaService.ObtenerPorId(id);
        if(acceso == null)
        {
            return NotFound();
        }

        return Ok(AccesoAtraccionDto.FromEntity(acceso));
    }

    [RequiereRol("Operador")]
    [HttpGet("reporte-uso")]
    public ActionResult<List<ReporteUsoAtraccionDto>> ObtenerReporteUsoAtracciones([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
    {
        var reporte = _consultaService.ObtenerReporteUsoAtracciones(fechaInicio, fechaFin);
        return Ok(reporte);
    }

    [RequiereRol("Visitante")]
    [HttpGet("mi-historial")]
    public ActionResult<List<AccesoAtraccionDto>> ObtenerMiHistorial([FromQuery] DateTime fecha)
    {
        var visitanteId = _authService.ObtenerUserIdDeClaims(User);
        var accesos = _consultaService.ObtenerAccesosPorVisitanteYFecha(visitanteId, fecha);
        var dtos = accesos.Select(AccesoAtraccionDto.FromEntity).ToList();
        return Ok(dtos);
    }

    [RequiereRol("Operador")]
    [HttpGet("visitante/{visitanteId}")]
    public ActionResult<List<AccesoAtraccionDto>> ObtenerAtraccionesVisitadas(Guid visitanteId, [FromQuery] DateTime fecha)
    {
        var accesos = _consultaService.ObtenerAccesosPorVisitanteYFecha(visitanteId, fecha);
        var dtos = accesos.Select(AccesoAtraccionDto.FromEntity).ToList();
        return Ok(dtos);
    }
}
