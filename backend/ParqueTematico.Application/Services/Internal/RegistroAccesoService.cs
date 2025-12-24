using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Dominio.Exceptions;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace ParqueTematico.Application.Services.Internal;

internal sealed class RegistroAccesoService(
    IAccesoAtraccionRepository accesoRepo,
    ITicketService ticketService,
    IAtraccionService atraccionService,
    IRelojService relojService)
{
    private readonly IAccesoAtraccionRepository _accesoRepo = accesoRepo;
    private readonly IAtraccionService _atraccionService = atraccionService;
    private readonly IRelojService _relojService = relojService;
    private readonly ITicketService _ticketService = ticketService;

    public Guid RegistrarIngreso(Ticket ticket, Atraccion atraccion, Visitante visitante)
    {
        DateTime fechaActual = _relojService.ObtenerFechaHora();
        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, fechaActual);
        _accesoRepo.Agregar(acceso);
        return acceso.Id;
    }

    public AccesoAtraccion RegistrarEgreso(Guid accesoId)
    {
        AccesoAtraccion? acceso = _accesoRepo.ObtenerPorId(accesoId);
        if(acceso == null)
        {
            throw new AccesoAtraccionException("El acceso no existe");
        }

        DateTime fechaActual = _relojService.ObtenerFechaHora();
        acceso.RegistrarEgreso(fechaActual);
        _accesoRepo.Actualizar(acceso);

        return acceso;
    }

    public Ticket ObtenerTicketValido(Guid ticketId)
    {
        Ticket? ticket = _ticketService.ObtenerPorId(ticketId);
        if(ticket == null)
        {
            throw new AccesoAtraccionException("El ticket no existe");
        }

        return ticket;
    }

    public Atraccion ObtenerAtraccionValida(Guid atraccionId)
    {
        Atraccion? atraccion = _atraccionService.ObtenerPorId(atraccionId);
        if(atraccion == null)
        {
            throw new AccesoAtraccionException("La atracción no existe");
        }

        return atraccion;
    }

    public Visitante ObtenerVisitanteDelTicket(Ticket ticket)
    {
        if(ticket.Visitante is not Visitante visitante)
        {
            throw new AccesoAtraccionException("El ticket no pertenece a un visitante válido");
        }

        return visitante;
    }
}
