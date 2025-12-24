using Dominio.Entities;

namespace Dtos;

public class TicketDto
{
    public Guid Id { get; set; }
    public UsuarioDTO Visitante { get; set; } = null!;
    public DateTime FechaVisita { get; set; }
    public string TipoEntrada { get; set; } = null!;
    public EventoDto? EventoAsociado { get; set; }

    public static TicketDto FromEntity(Ticket ticket)
    {
        return new TicketDto
        {
            Id = ticket.Id,
            Visitante = new UsuarioDTO(
                ticket.Visitante.Id,
                ticket.Visitante.Nombre,
                ticket.Visitante.Apellido,
                ticket.Visitante.Email,
                "Visitante",
                ticket.Visitante.FechaNacimiento,
                nameof(ticket.Visitante.NivelMembresia),
                ticket.Visitante.PuntosActuales),
            FechaVisita = ticket.FechaVisita,
            TipoEntrada = ticket.TipoEntrada.ToString(),
            EventoAsociado =
                ticket.EventoAsociado != null ? EventoDto.FromEntity(ticket.EventoAsociado) : null,
        };
    }
}
