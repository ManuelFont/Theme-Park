using Dtos;
using Microsoft.AspNetCore.Mvc;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Filtros;

namespace ParqueTematico.WebApi.Controllers;

[ApiController]
[Route("api/ranking")]
[RequiereRol("Administrador")]
public class RankingController(IRankingService rankingService) : ControllerBase
{
    private readonly IRankingService _rankingService = rankingService;

    [HttpGet("estrategias")]
    public IActionResult ListarEstrategias()
    {
        var estrategias = _rankingService.ObtenerEstrategiasDisponibles();
        return Ok(estrategias);
    }

    [HttpGet("estrategia-activa")]
    public IActionResult ObtenerEstrategiaActiva()
    {
        var nombreEstrategia = _rankingService.ObtenerNombreEstrategiaActiva();
        return Ok(new { estrategiaActiva = nombreEstrategia });
    }

    [HttpPut("estrategia-activa")]
    public IActionResult CambiarEstrategia([FromBody] CambiarEstrategiaRequest request)
    {
        var resultado = _rankingService.CambiarEstrategia(request.NombreEstrategia);

        if(!resultado)
        {
            return BadRequest(new { mensaje = "Estrategia no válida o no encontrada" });
        }

        var nombreEstrategia = _rankingService.ObtenerNombreEstrategiaActiva();
        return Ok(new { mensaje = "Estrategia actualizada correctamente", estrategiaActiva = nombreEstrategia });
    }

    [HttpGet("diario")]
    public IActionResult ObtenerRankingDiario()
    {
        var ranking = _rankingService.ObtenerRankingDiario(10);

        var resultado = ranking.Select((r, index) => new
        {
            posicion = index + 1,
            visitanteId = r.VisitanteId,
            nombreCompleto = r.NombreVisitante,
            puntosTotales = r.PuntosTotales
        });

        return Ok(resultado);
    }
}
