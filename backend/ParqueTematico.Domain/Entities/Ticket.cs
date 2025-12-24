using Dominio.Entities.Usuarios;
using Dominio.Exceptions;

namespace Dominio.Entities;

/// <summary>
///     Tipo de entrada para los eventos.
/// </summary>
public enum TipoEntrada
{
    /// <summary>
    ///     Ticket generico.
    /// </summary>
    General,

    /// <summary>
    ///     Ticker para eventos especiales.
    /// </summary>
    EventoEspecial
}

public class Ticket
{
    protected Ticket()
    {
    }

    public Ticket(Visitante visitante, DateTime fechaVisita, TipoEntrada tipoEntrada, Evento? eventoAsociado)
    {
        if(tipoEntrada == TipoEntrada.EventoEspecial && eventoAsociado == null)
        {
            throw new TicketException("El evento asociado no puede ser nulo para un ticket de tipo evento especial");
        }

        Visitante = visitante ?? throw new TicketException("El visitante no puede ser nulo");
        FechaVisita = fechaVisita;
        TipoEntrada = tipoEntrada;
        EventoAsociado = eventoAsociado;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public Visitante Visitante { get; private set; } = null!;
    public DateTime FechaVisita { get; private set; }
    public TipoEntrada TipoEntrada { get; }
    public Evento? EventoAsociado { get; }

    public void CambiarFechaVisita(DateTime nuevaFecha)
    {
        if(nuevaFecha < DateTime.Now)
        {
            throw new TicketException("La fecha de visita no puede ser anterior a la actual");
        }

        FechaVisita = nuevaFecha;
    }

    public bool EsValidoParaFecha(DateTime fecha)
    {
        return FechaVisita.Date == fecha.Date;
    }

    public bool IncluyeAtraccion(Guid atraccionId)
    {
        if(TipoEntrada == TipoEntrada.General)
        {
            return true;
        }

        if(EventoAsociado == null)
        {
            return false;
        }

        return EventoAsociado.Atracciones.Any(a => a.Id == atraccionId);
    }
}
