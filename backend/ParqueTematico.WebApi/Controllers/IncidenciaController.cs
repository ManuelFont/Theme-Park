using Dtos;
using Microsoft.AspNetCore.Mvc;
using ParqueTematico.Application.Services;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Filtros;

namespace ParqueTematico.WebApi.Controllers;

[ApiController]
[Route("api/incidencias")]
[RequiereRol("Administrador", "Operador")]
public class IncidenciaController(IncidenciaService service) : ControllerBase
{
    private readonly IIncidenciaService _service = service;

    [HttpPost]
    public ActionResult<IncidenciaDto> Crear([FromBody] CrearIncidenciaRequest request)
    {
        var incidencia = _service.Crear(
            request.AtraccionId,
            request.TipoIncidencia,
            request.Descripcion);
        var dto = IncidenciaDto.FromEntity(incidencia);
        return Created(string.Empty, dto);
    }

    [HttpPut("{id}/cerrar")]
    public IActionResult Cerrar(Guid id)
    {
        _service.Cerrar(id);
        return NoContent();
    }

    [HttpGet]
    public ActionResult<List<IncidenciaDto>> ObtenerActivasPorAtraccion(
        [FromQuery] Guid atraccionId,
        [FromQuery] bool activas = false)
    {
        if(activas)
        {
            var incidencias = _service.ObtenerActivasPorAtraccion(atraccionId);
            var dtos = incidencias.Select(IncidenciaDto.FromEntity).ToList();
            return Ok(dtos);
        }

        return Ok(new List<IncidenciaDto>());
    }

    [HttpGet("existe-activa")]
    public ActionResult<bool> ExisteActiva([FromQuery] Guid atraccionId)
    {
        var existe = _service.ExisteActiva(atraccionId);
        return Ok(existe);
    }
}
