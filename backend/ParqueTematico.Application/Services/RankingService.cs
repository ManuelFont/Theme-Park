using Dominio.Entities;
using Dominio.Entities.Puntuacion;
using Dominio.Entities.Usuarios;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace ParqueTematico.Application.Services;

public class RankingService(
    IAccesoAtraccionRepository accesoRepo,
    IRelojService relojService,
    IEstrategiaService estrategiaService) : IRankingService
{
    private readonly IAccesoAtraccionRepository _accesoRepo = accesoRepo;
    private readonly IRelojService _relojService = relojService;
    private readonly IEstrategiaService _estrategiaService = estrategiaService;

    public IPuntuacion ObtenerEstrategiaActiva()
    {
        return _estrategiaService.ObtenerEstrategiaActiva();
    }

    public string ObtenerNombreEstrategiaActiva()
    {
        return _estrategiaService.ObtenerEstrategiaActiva().Nombre;
    }

    public bool CambiarEstrategia(string nombreEstrategia)
    {
        return _estrategiaService.CambiarEstrategia(nombreEstrategia);
    }

    public IEnumerable<(string Nombre, string Descripcion)> ObtenerEstrategiasDisponibles()
    {
        return _estrategiaService.ObtenerEstrategiasDisponibles();
    }

    public IEnumerable<(Guid VisitanteId, string NombreVisitante, int PuntosTotales)> ObtenerRankingDiario(int top = 10)
    {
        DateTime fechaActual = _relojService.ObtenerFechaHora();
        DateTime inicioDelDia = fechaActual.Date;
        DateTime finDelDia = fechaActual.Date.AddDays(1).AddTicks(-1);
        IList<AccesoAtraccion> accesosDelDia = _accesoRepo.ObtenerAccesosEntreFechas(inicioDelDia, finDelDia);

        var visitantesConPuntos = new List<(Guid VisitanteId, string NombreVisitante, int PuntosTotales)>();

        IEnumerable<IGrouping<Guid, AccesoAtraccion>> accesosAgrupadosPorVisitante =
            accesosDelDia.GroupBy(a => a.Visitante.Id);

        foreach(IGrouping<Guid, AccesoAtraccion> grupo in accesosAgrupadosPorVisitante)
        {
            Guid visitanteId = grupo.Key;
            var accesos = grupo.ToList();
            Visitante visitante = accesos.First().Visitante;

            var nombreCompleto = $"{visitante.Nombre} {visitante.Apellido}";
            var puntos = CalcularPuntosTotales(accesos, accesosDelDia);

            visitantesConPuntos.Add((visitanteId, nombreCompleto, puntos));
        }

        return visitantesConPuntos
            .OrderByDescending(v => v.PuntosTotales)
            .Take(top);
    }

    private int CalcularPuntosTotales(IEnumerable<AccesoAtraccion> accesosVisitante,
        IEnumerable<AccesoAtraccion> todosLosAccesos)
    {
        var total = 0;
        var estrategiaActiva = _estrategiaService.ObtenerEstrategiaActiva();

        foreach(AccesoAtraccion acceso in accesosVisitante)
        {
            total += estrategiaActiva.CalcularPuntos(acceso, todosLosAccesos);
        }

        return total;
    }
}
