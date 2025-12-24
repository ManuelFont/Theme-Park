using Dtos;
using Microsoft.AspNetCore.Mvc;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Filtros;

namespace ParqueTematico.WebApi.Controllers;

[ApiController]
[Route("api/mantenimientos")]
[RequiereRol("Administrador")]
public class MantenimientoPreventivoController(IMantenimientoPreventivoService service)
    : ControllerBase
{
    private readonly IMantenimientoPreventivoService _service = service;

    [HttpPost]
    public ActionResult<MantenimientoPreventivoDto> CrearMantenimiento(
        [FromBody] CrearMantenimientoPreventivoRequest request)
    {
        var mantenimiento = _service.Crear(
            request.AtraccionId,
            request.Descripcion,
            request.FechaInicio,
            request.FechaFin);
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = mantenimiento.Id },
            MantenimientoPreventivoDto.FromEntity(mantenimiento));
    }

    [HttpGet("{id}")]
    public ActionResult<MantenimientoPreventivoDto> ObtenerPorId(Guid id)
    {
        var mantenimiento = _service.ObtenerPorId(id);

        if(mantenimiento == null)
        {
            return NotFound();
        }

        return Ok(MantenimientoPreventivoDto.FromEntity(mantenimiento));
    }

    [HttpGet]
    public ActionResult<IEnumerable<MantenimientoPreventivoDto>> ObtenerTodos()
    {
        var mantenimientos = _service.ObtenerTodos();
        return Ok(mantenimientos.Select(MantenimientoPreventivoDto.FromEntity));
    }

    [HttpPut("{id}/finalizar")]
    public IActionResult Finalizar(Guid id)
    {
        _service.Cerrar(id);
        return NoContent();
    }
}
