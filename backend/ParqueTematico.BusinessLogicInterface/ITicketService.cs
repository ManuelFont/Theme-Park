using Dominio.Entities;

namespace ParqueTematico.BusinessLogicInterface;

public interface ITicketService
{
    Guid ComprarTicket(Guid visitanteId, DateTime fechaVisita, TipoEntrada tipoEntrada, Guid? eventoId);

    Ticket? ObtenerPorId(Guid id);

    IEnumerable<Ticket> ObtenerTicketsPorVisitante(Guid visitanteId);
}
