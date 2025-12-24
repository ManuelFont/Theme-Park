using Dtos;
using Microsoft.AspNetCore.Mvc;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Filtros;

namespace ParqueTematico.WebApi.Controllers;

[ApiController]
[Route("api/relojes")]
public class RelojController(IRelojService service) : ControllerBase
{
    private readonly IRelojService _service = service;

    [RequiereRol("Administrador")]
    [HttpPut]
    public ActionResult<RelojDto> ModificarFechaHora([FromBody] RelojDto request)
    {
        var reloj = _service.ModificarFechaHora(request.FechaHora);
        return Ok(new RelojDto(reloj.FechaHora));
    }

    [HttpGet]
    public ActionResult<RelojDto> GetFechaHoraReloj()
    {
        var fechaHora = _service.ObtenerFechaHora();

        return Ok(new RelojDto(fechaHora));
    }
}
