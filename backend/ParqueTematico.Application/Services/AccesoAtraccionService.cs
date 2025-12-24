using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Dtos;
using ParqueTematico.Application.Services.Internal;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace ParqueTematico.Application.Services;

public class AccesoAtraccionService(
    IAccesoAtraccionRepository accesoRepo,
    ITicketService ticketService,
    IAtraccionService atraccionService,
    IRelojService relojService,
    IIncidenciaService incidenciaService,
    IRankingService rankingService,
    IHistorialPuntuacionService historialServiceService,
    IUsuarioRepository usuarioRepo) : IAccesoAtraccionService, IComandoAccesoAtraccion, IConsultaAccesoAtraccion
{
    private readonly IAccesoAtraccionRepository _accesoRepo = accesoRepo;
    private readonly ValidacionAccesoService _validacionService = new(incidenciaService, accesoRepo);
    private readonly RegistroAccesoService _registroService = new(accesoRepo, ticketService, atraccionService, relojService);
    private readonly GestionPuntosAccesoService _gestionPuntosService = new(accesoRepo, rankingService, historialServiceService, usuarioRepo);
    private readonly ReporteAccesoService _reporteService = new(accesoRepo);

    public Guid RegistrarIngreso(Guid ticketId, Guid atraccionId)
    {
        Ticket ticket = _registroService.ObtenerTicketValido(ticketId);
        Atraccion atraccion = _registroService.ObtenerAtraccionValida(atraccionId);
        Visitante visitante = _registroService.ObtenerVisitanteDelTicket(ticket);

        _validacionService.ValidarAcceso(ticket, atraccion, visitante, DateTime.Now, atraccionId);

        return _registroService.RegistrarIngreso(ticket, atraccion, visitante);
    }

    public void RegistrarEgreso(Guid accesoId)
    {
        AccesoAtraccion acceso = _registroService.RegistrarEgreso(accesoId);
        _gestionPuntosService.RegistrarHistorialPuntos(acceso);
    }

    public void AsignarPuntos(Guid accesoId, int puntos)
    {
        _gestionPuntosService.AsignarPuntos(accesoId, puntos);
    }

    public IEnumerable<AccesoAtraccion> ObtenerAccesosEntreFechas(DateTime fechaInicio, DateTime fechaFin)
    {
        return _accesoRepo.ObtenerAccesosEntreFechas(fechaInicio, fechaFin);
    }

    public int ObtenerAforoActual(Guid atraccionId)
    {
        IList<AccesoAtraccion> accesosActivos = _accesoRepo.ObtenerAccesosSinEgresoPorAtraccion(atraccionId);
        return accesosActivos.Count;
    }

    public AccesoAtraccion? ObtenerPorId(Guid id)
    {
        return _accesoRepo.ObtenerPorId(id);
    }

    public IEnumerable<AccesoAtraccion> ObtenerAccesosPorVisitanteYFecha(Guid visitanteId, DateTime fecha)
    {
        return _accesoRepo.ObtenerAccesosPorVisitanteYFecha(visitanteId, fecha);
    }

    public List<ReporteUsoAtraccionDto> ObtenerReporteUsoAtracciones(DateTime fechaInicio, DateTime fechaFin)
    {
        return _reporteService.ObtenerReporteUsoAtracciones(fechaInicio, fechaFin);
    }
}
