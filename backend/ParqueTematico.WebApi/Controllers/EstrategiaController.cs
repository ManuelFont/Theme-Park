using Dtos;
using Microsoft.AspNetCore.Mvc;
using ParqueTematico.BusinessLogicInterface;

namespace ParqueTematico.WebApi.Controllers;

[ApiController]
[Route("api/estrategias")]
public class EstrategiaController(IEstrategiaService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<EstrategiaDto>> ObtenerTodas()
    {
        var estrategias = service.ObtenerEstrategiasDisponibles()
            .Select(e => new EstrategiaDto
            {
                Nombre = e.Nombre,
                Descripcion = e.Descripcion
            });

        return Ok(estrategias);
    }

    [HttpGet("activa")]
    public ActionResult<EstrategiaDto> ObtenerActiva()
    {
        var estrategia = service.ObtenerEstrategiaActiva();

        return Ok(new EstrategiaDto
        {
            Nombre = estrategia.GetType().Name,
            Descripcion = estrategia.Descripcion
        });
    }

    [HttpPut("activa")]
    public IActionResult CambiarEstrategia([FromBody] CambiarEstrategiaRequest request)
    {
        var exito = service.CambiarEstrategia(request.NombreEstrategia);

        if(!exito)
        {
            return BadRequest($"La estrategia '{request.NombreEstrategia}' no existe.");
        }

        return NoContent();
    }
}
