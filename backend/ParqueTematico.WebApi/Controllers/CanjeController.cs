using Dtos;
using Microsoft.AspNetCore.Mvc;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Filtros;

namespace ParqueTematico.WebApi.Controllers;

[ApiController]
[Route("api/canje")]
public class CanjeController(ICanjeService service, IAuthService authService) : ControllerBase
{
    private readonly ICanjeService _service = service;
    private readonly IAuthService _authService = authService;

    [RequiereRol("Visitante")]
    [HttpGet]
    public ActionResult<IList<CanjeCreadoDto>> ObtenerMisCanjes()
    {
        var visitanteId = _authService.ObtenerUserIdDeClaims(User);
        var canjes = _service.ObtenerPorVisitante(visitanteId);
        return Ok(canjes);
    }

    [HttpGet("{id}")]
    public ActionResult<CanjeCreadoDto> Obtener(Guid id)
    {
        var canje = _service.ObtenerCanje(id);
        return Ok(canje);
    }

    [RequiereRol("Visitante")]
    [HttpPost]
    public ActionResult<CanjeCreadoDto> Crear([FromBody] CanjearRecompensaRequest request)
    {
        var visitanteId = _authService.ObtenerUserIdDeClaims(User);
        var dto = new CanjeCrearDto
        {
            UsuarioId = visitanteId,
            RecompensaId = request.RecompensaId
        };
        var creada = _service.CrearCanje(dto);
        return CreatedAtAction(nameof(Obtener), new { id = creada.Id }, creada);
    }

    [HttpPut("{id}")]
    public ActionResult<CanjeCreadoDto> Actualizar(Guid id, [FromBody] CanjeCrearDto dto)
    {
        var actualizada = _service.ActualizarCanje(id, dto);
        return Ok(actualizada);
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(Guid id)
    {
        _service.EliminarCanje(id);
        return NoContent();
    }
}
