using Dtos;
using Microsoft.AspNetCore.Mvc;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Filtros;

namespace ParqueTematico.WebApi.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketController(ITicketService service, IAuthService authService) : ControllerBase
{
    private readonly ITicketService _service = service;
    private readonly IAuthService _authService = authService;

    [RequiereRol("Visitante")]
    [HttpPost]
    public IActionResult ComprarTicket([FromBody] ComprarTicketRequest request)
    {
        var visitanteId = _authService.ObtenerUserIdDeClaims(User);
        var ticketId = _service.ComprarTicket(visitanteId, request.FechaVisita, request.TipoEntrada, request.EventoId);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = ticketId }, new { ticketId });
    }

    [RequiereRol("Operador")]
    [HttpGet("{id}")]
    public ActionResult<TicketDto> ObtenerPorId(Guid id)
    {
        var ticket = _service.ObtenerPorId(id);
        if(ticket == null)
        {
            return NotFound();
        }

        return Ok(TicketDto.FromEntity(ticket));
    }

    [RequiereRol("Operador")]
    [HttpGet("visitante/{visitanteId}")]
    public ActionResult<List<TicketDto>> ObtenerPorVisitante(Guid visitanteId)
    {
        var tickets = _service.ObtenerTicketsPorVisitante(visitanteId);
        var dtos = tickets.Select(TicketDto.FromEntity).ToList();
        return Ok(dtos);
    }
}
