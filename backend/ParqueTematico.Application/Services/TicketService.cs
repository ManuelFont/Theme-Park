using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Dominio.Exceptions;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace ParqueTematico.Application.Services;

public class TicketService(
    ITicketRepository ticketRepo,
    IUsuarioRepository usuarioRepo,
    IEventoService eventoService,
    IRelojService relojService) : ITicketService
{
    private readonly IEventoService _eventoService = eventoService;
    private readonly IRelojService _relojService = relojService;
    private readonly ITicketRepository _ticketRepo = ticketRepo;
    private readonly IUsuarioRepository _usuarioRepo = usuarioRepo;

    public Guid ComprarTicket(Guid visitanteId, DateTime fechaVisita, TipoEntrada tipoEntrada, Guid? eventoId)
    {
        DateTime fechaActual = _relojService.ObtenerFechaHora();

        if(fechaVisita.Date < fechaActual.Date)
        {
            throw new TicketException("La fecha de visita no puede ser anterior a la actual");
        }

        Usuario? usuario = _usuarioRepo.ObtenerPorId(visitanteId);
        if(usuario is not Visitante visitante)
        {
            throw new TicketException("El usuario no es un visitante válido");
        }

        if(tipoEntrada == TipoEntrada.EventoEspecial && !eventoId.HasValue)
        {
            throw new TicketException("Debe especificar un evento para tickets de tipo EventoEspecial");
        }

        if(tipoEntrada == TipoEntrada.General && eventoId.HasValue)
        {
            throw new TicketException("Los tickets generales no pueden estar asociados a un evento");
        }

        Evento? evento = null;
        if(eventoId.HasValue)
        {
            evento = _eventoService.ObtenerPorId(eventoId.Value);
            if(evento == null)
            {
                throw new TicketException("El evento no existe");
            }

            var ticketsVendidos = _ticketRepo.ObtenerTodos()
                .Count(t => t.EventoAsociado != null && t.EventoAsociado.Id == eventoId.Value);

            if(ticketsVendidos >= evento.Aforo)
            {
                throw new TicketException("El evento ha alcanzado su aforo máximo");
            }
        }

        var ticket = new Ticket(visitante, fechaVisita, tipoEntrada, evento);
        _ticketRepo.Agregar(ticket);

        return ticket.Id;
    }

    public Ticket? ObtenerPorId(Guid id)
    {
        return _ticketRepo.ObtenerPorId(id);
    }

    public IEnumerable<Ticket> ObtenerTicketsPorVisitante(Guid visitanteId)
    {
        return _ticketRepo.ObtenerTodos().Where(t => t.Visitante.Id == visitanteId);
    }
}
