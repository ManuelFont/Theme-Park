using Dtos;
using Microsoft.AspNetCore.Mvc;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Filtros;

namespace ParqueTematico.WebApi.Controllers;

[ApiController]
[Route("api/eventos")]
public class EventoController(IEventoService service) : ControllerBase
{
    private readonly IEventoService _service = service;

    [RequiereRol("Administrador")]
    [HttpPost]
    public IActionResult Crear([FromBody] CrearEventoRequest request)
    {
        var evento = _service.CrearEvento(
            request.Nombre,
            request.Fecha,
            request.Hora,
            request.Aforo,
            request.CostoAdicional);

        return CreatedAtAction(nameof(ObtenerPorId), new { id = evento.Id }, EventoDto.FromEntity(evento));
    }

    [HttpGet("{id}")]
    public ActionResult<EventoDto> ObtenerPorId(Guid id)
    {
        var evento = _service.ObtenerPorId(id);

        if(evento is null)
        {
            return NotFound();
        }

        return Ok(EventoDto.FromEntity(evento));
    }

    [RequiereRol("Administrador")]
    [HttpDelete("{id}")]
    public IActionResult Eliminar(Guid id)
    {
        _service.Eliminar(id);
        return NoContent();
    }

    [HttpGet]
    public ActionResult<List<EventoDto>> ObtenerTodos()
    {
        var eventos = _service.ObtenerTodos();
        var dtos = eventos.Select(EventoDto.FromEntity).ToList();
        return Ok(dtos);
    }

    [RequiereRol("Administrador")]
    [HttpPost("{eventoId}/atracciones/{atraccionId}")]
    public IActionResult AgregarAtraccion(Guid eventoId, Guid atraccionId)
    {
        _service.AgregarAtraccionAEvento(eventoId, atraccionId);
        return Ok(new { mensaje = "Atracción agregada al evento exitosamente" });
    }

    [RequiereRol("Administrador")]
    [HttpDelete("{eventoId}/atracciones/{atraccionId}")]
    public IActionResult EliminarAtraccion(Guid eventoId, Guid atraccionId)
    {
        _service.EliminarAtraccionDeEvento(eventoId, atraccionId);
        return NoContent();
    }
}
