using Dominio.Entities;
namespace Dtos;

public class AccesoAtraccionDto
{
    public Guid Id { get; set; }
    public UsuarioDTO Visitante { get; set; } = null!;
    public AtraccionDto Atraccion { get; set; } = null!;
    public Guid TicketId { get; set; }
    public DateTime FechaHoraIngreso { get; set; }
    public DateTime? FechaHoraEgreso { get; set; }
    public int PuntosObtenidos { get; set; }

    public static AccesoAtraccionDto FromEntity(AccesoAtraccion acceso)
    {
        return new AccesoAtraccionDto
        {
            Id = acceso.Id,
            Visitante = new UsuarioDTO(acceso.Visitante.Id, acceso.Visitante.Nombre, acceso.Visitante.Apellido, acceso.Visitante.Email, "Visitante", acceso.Visitante.FechaNacimiento, nameof(acceso.Visitante.NivelMembresia), acceso.Visitante.PuntosActuales),
            Atraccion = AtraccionDto.FromEntity(acceso.Atraccion),
            TicketId = acceso.Ticket.Id,
            FechaHoraIngreso = acceso.FechaHoraIngreso,
            FechaHoraEgreso = acceso.FechaHoraEgreso,
            PuntosObtenidos = acceso.PuntosObtenidos
        };
    }

    public AccesoAtraccion ToEntity(Dominio.Entities.Usuarios.Visitante visitante, Atraccion atraccion, Ticket ticket)
    {
        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, FechaHoraIngreso);

        if(FechaHoraEgreso.HasValue)
        {
            acceso.RegistrarEgreso(FechaHoraEgreso.Value);
        }

        if(PuntosObtenidos > 0)
        {
            acceso.AsignarPuntos(PuntosObtenidos);
        }

        return acceso;
    }
}
