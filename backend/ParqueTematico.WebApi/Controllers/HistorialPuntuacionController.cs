using Dtos;
using Microsoft.AspNetCore.Mvc;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Filtros;

namespace ParqueTematico.WebApi.Controllers;

[ApiController]
[Route("api/historial-puntuacion")]
[RequiereRol("Visitante")]
public class HistorialPuntuacionController(IHistorialPuntuacionService serviceService, IAuthService authService) : ControllerBase
{
    private readonly IHistorialPuntuacionService _serviceService = serviceService;
    private readonly IAuthService _authService = authService;

    [HttpGet("visitante/{visitanteId}")]
    public ActionResult<List<HistorialPuntuacionDto>> ObtenerHistorialPorVisitante(Guid visitanteId)
    {
        var historial = _serviceService.ObtenerHistorialPorVisitante(visitanteId);

        var historialesDto = historial
            .Select(HistorialPuntuacionDto.FromEntity)
            .ToList();

        return Ok(historialesDto);
    }

    [HttpGet("mi-historial")]
    public ActionResult<List<HistorialPuntuacionDto>> ObtenerMiHistorial()
    {
        var userId = _authService.ObtenerUserIdDeClaims(User);
        var historial = _serviceService.ObtenerHistorialPorVisitante(userId);

        var historialesDto = historial
            .Select(HistorialPuntuacionDto.FromEntity)
            .ToList();

        return Ok(historialesDto);
    }
}
