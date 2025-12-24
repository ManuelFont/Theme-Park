using Dtos;
using Microsoft.AspNetCore.Mvc;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Filtros;

namespace ParqueTematico.WebApi.Controllers;

[ApiController]
[Route("api/recompensa")]
public class RecompensaController(IRecompensaService service) : ControllerBase
{
    private readonly IRecompensaService _service = service;

    [HttpGet]
    public ActionResult<IList<RecompensaCreadoDto>> ObtenerTodos()
    {
        var recompensas = _service.ObtenerTodos();
        return Ok(recompensas);
    }

    [HttpGet("{id}")]
    public ActionResult<RecompensaCreadoDto> Obtener(Guid id)
    {
        var recompensa = _service.ObtenerRecompensa(id);
        return Ok(recompensa);
    }

    [HttpPost]
    [RequiereRol("Administrador")]
    public ActionResult<RecompensaCreadoDto> Crear([FromBody] RecompensaCrearDto dto)
    {
        var creada = _service.CrearRecompensa(dto);
        return CreatedAtAction(nameof(Obtener), new { id = creada.Id }, creada);
    }

    [HttpPut("{id}")]
    [RequiereRol("Administrador")]
    public ActionResult<RecompensaCreadoDto> Actualizar(Guid id, [FromBody] RecompensaCrearDto dto)
    {
        var actualizada = _service.ActualizarRecompensa(id, dto);
        return Ok(actualizada);
    }

    [HttpDelete("{id}")]
    [RequiereRol("Administrador")]
    public IActionResult Eliminar(Guid id)
    {
        _service.EliminarRecompensa(id);
        return NoContent();
    }
}
