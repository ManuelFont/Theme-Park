using Dominio.Entities;

namespace Dtos;

public class ComprarTicketRequest
{
    public Guid VisitanteId { get; set; }
    public DateTime FechaVisita { get; set; }
    public TipoEntrada TipoEntrada { get; set; }
    public Guid? EventoId { get; set; }
}
