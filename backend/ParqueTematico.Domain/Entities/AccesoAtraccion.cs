using Dominio.Entities.Usuarios;
using Dominio.Exceptions;

namespace Dominio.Entities;

public class AccesoAtraccion
{
    private AccesoAtraccion()
    {
    }

    public AccesoAtraccion(Visitante visitante, Atraccion atraccion, Ticket ticket, DateTime fechaHoraIngreso)
    {
        if(visitante == null)
        {
            throw new AccesoAtraccionException("El visitante no puede ser nulo");
        }

        if(atraccion == null)
        {
            throw new AccesoAtraccionException("La atracción no puede ser nula");
        }

        if(ticket == null)
        {
            throw new AccesoAtraccionException("El ticket no puede ser nulo");
        }

        Visitante = visitante;
        Atraccion = atraccion;
        AtraccionId = atraccion.Id;
        Ticket = ticket;
        FechaHoraIngreso = fechaHoraIngreso;
        FechaHoraEgreso = null;
        PuntosObtenidos = 0;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Visitante Visitante { get; private set; } = null!;
    public Atraccion Atraccion { get; private set; } = null!;
    public Guid AtraccionId { get; private set; }
    public Ticket Ticket { get; private set; } = null!;
    public DateTime FechaHoraIngreso { get; }
    public DateTime? FechaHoraEgreso { get; private set; }
    public int PuntosObtenidos { get; private set; }

    public bool EstaActivo => !FechaHoraEgreso.HasValue;

    public TimeSpan? TiempoPermanencia
    {
        get
        {
            if(!FechaHoraEgreso.HasValue)
            {
                return null;
            }

            return FechaHoraEgreso.Value - FechaHoraIngreso;
        }
    }

    public void RegistrarEgreso(DateTime fechaHoraEgreso)
    {
        if(FechaHoraEgreso.HasValue)
        {
            throw new AccesoAtraccionException("El egreso ya fue registrado");
        }

        if(fechaHoraEgreso < FechaHoraIngreso)
        {
            throw new AccesoAtraccionException("La fecha de egreso no puede ser anterior a la de ingreso");
        }

        FechaHoraEgreso = fechaHoraEgreso;
    }

    public void AsignarPuntos(int puntos)
    {
        if(puntos < 0)
        {
            throw new AccesoAtraccionException("Los puntos no pueden ser negativos");
        }

        PuntosObtenidos = puntos;
    }
}
