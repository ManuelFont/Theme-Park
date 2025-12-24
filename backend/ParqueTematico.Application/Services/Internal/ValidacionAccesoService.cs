using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Dominio.Exceptions;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace ParqueTematico.Application.Services.Internal;

internal sealed class ValidacionAccesoService(IIncidenciaService incidenciaService, IAccesoAtraccionRepository accesoRepo)
{
    private readonly IAccesoAtraccionRepository _accesoRepo = accesoRepo;
    private readonly IIncidenciaService _incidenciaService = incidenciaService;

    public void ValidarAcceso(Ticket ticket, Atraccion atraccion, Visitante visitante, DateTime fechaActual,
        Guid atraccionId)
    {
        ValidarFechaTicket(ticket, fechaActual);
        ValidarAtraccionEnTicket(ticket, atraccionId);
        ValidarEdadVisitante(atraccion, visitante, fechaActual);
        ValidarIncidencias(atraccionId);
        ValidarAforo(atraccion);
    }

    private void ValidarFechaTicket(Ticket ticket, DateTime fechaActual)
    {
        if(!ticket.EsValidoParaFecha(fechaActual))
        {
            throw new AccesoAtraccionException("El ticket no es válido para la fecha actual");
        }
    }

    private void ValidarAtraccionEnTicket(Ticket ticket, Guid atraccionId)
    {
        if(!ticket.IncluyeAtraccion(atraccionId))
        {
            throw new AccesoAtraccionException("El ticket no incluye acceso a esta atracción");
        }
    }

    private void ValidarEdadVisitante(Atraccion atraccion, Visitante visitante, DateTime fechaActual)
    {
        if(!atraccion.PuedeIngresarVisitante(visitante, fechaActual))
        {
            throw new AccesoAtraccionException("El visitante no cumple con la edad mínima requerida");
        }
    }

    private void ValidarIncidencias(Guid atraccionId)
    {
        if(_incidenciaService.ExisteActiva(atraccionId))
        {
            throw new AccesoAtraccionException("La atracción tiene una incidencia activa y no permite ingresos");
        }
    }

    private void ValidarAforo(Atraccion atraccion)
    {
        var aforoActual = ObtenerAforoActual(atraccion.Id);
        if(aforoActual >= atraccion.CapacidadMaxima)
        {
            throw new AccesoAtraccionException("La atracción ha alcanzado su capacidad máxima");
        }
    }

    private int ObtenerAforoActual(Guid atraccionId)
    {
        IList<AccesoAtraccion> accesosActivos = _accesoRepo.ObtenerAccesosSinEgresoPorAtraccion(atraccionId);
        return accesosActivos.Count;
    }
}
