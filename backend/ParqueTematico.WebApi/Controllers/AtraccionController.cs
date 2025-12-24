using Dtos;
using Microsoft.AspNetCore.Mvc;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Filtros;

namespace ParqueTematico.WebApi.Controllers;

[ApiController]
[Route("api/atracciones")]
public class AtraccionController(IAtraccionService service, IAccesoAtraccionService accesoService) : ControllerBase
{
    private readonly IAtraccionService _service = service;

    [HttpPost]
    [RequiereRol("Administrador")]
    public IActionResult Crear([FromBody] AtraccionDto dto)
    {
        _service.Crear(
                dto.Nombre,
                dto.Tipo,
                dto.EdadMinima,
                dto.CapacidadMaxima,
                dto.Descripcion,
                dto.Disponible);
        return Ok();
    }

    [HttpGet("{id}")]
    [RequiereRol("Administrador", "Operador")]
    public ActionResult<AtraccionDto> ObtenerPorId(Guid id)
    {
        var atraccion = _service.ObtenerPorId(id);
        if(atraccion == null)
        {
            return NotFound();
        }

        return Ok(AtraccionDto.FromEntity(atraccion));
    }

    [HttpGet]
    [RequiereRol("Administrador", "Operador")]
    public ActionResult<List<AtraccionDto>> ObtenerTodos()
    {
        var atracciones = _service.ObtenerTodos();
        var dtos = atracciones.Select(AtraccionDto.FromEntity).ToList();
        return Ok(dtos);
    }

    [HttpPut("{id}")]
    [RequiereRol("Administrador")]
    public IActionResult Actualizar(Guid id, [FromBody] AtraccionDto dto)
    {
        dto.Id = id;
        _service.Actualizar(dto);
        return Ok();
    }

    [HttpDelete("{id}")]
    [RequiereRol("Administrador")]
    public IActionResult Eliminar(Guid id)
    {
        _service.Eliminar(id);
        return Ok();
    }

    [HttpGet("{id}/aforo")]
    [RequiereRol("Operador")]
    public ActionResult<AforoDto> ObtenerAforo(Guid id)
    {
        var atraccion = _service.ObtenerPorId(id);
        if(atraccion == null)
        {
            return NotFound("La atracción no existe");
        }

        var aforoActual = accesoService.ObtenerAforoActual(id);

        return Ok(new AforoDto
        {
            AtraccionId = id,
            CapacidadMaxima = atraccion.CapacidadMaxima,
            AforoActual = aforoActual,
            Restante = atraccion.CapacidadMaxima - aforoActual
        });
    }
}
