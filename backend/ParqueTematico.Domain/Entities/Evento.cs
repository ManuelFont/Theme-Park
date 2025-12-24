using Dominio.Exceptions;

namespace Dominio.Entities;

public class Evento
{
    public Evento(string nombre, DateTime fecha, TimeSpan hora, int aforo, decimal costoAdicional)
    {
        if(string.IsNullOrEmpty(nombre))
        {
            throw new EventoException("El nombre del evento no puede ser vacío");
        }

        if(fecha < DateTime.Now)
        {
            throw new EventoException("La fecha del evento no puede ser anterior a la actual");
        }

        if(aforo < 1)
        {
            throw new EventoException("El aforo del evento no puede ser cero o negativo");
        }

        if(costoAdicional < 0)
        {
            throw new EventoException("El costo adicional del evento no puede ser negativo");
        }

        Nombre = nombre;
        Fecha = fecha;
        Hora = hora;
        Aforo = aforo;
        CostoAdicional = costoAdicional;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nombre { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public int Aforo { get; set; }
    public decimal CostoAdicional { get; set; }
    public List<Atraccion> Atracciones { get; set; } = [];

    public void AgregarAtraccion(Atraccion atraccion)
    {
        if(atraccion == null)
        {
            throw new EventoException("La atracción no puede ser nula");
        }

        if(YaContieneAtraccion(atraccion.Id))
        {
            throw new EventoException("La atracción ya está asignada a este evento");
        }

        Atracciones.Add(atraccion);
    }

    public void EliminarAtraccion(Guid atraccionId)
    {
        Atraccion? atraccion = BuscarAtraccion(atraccionId);
        if(atraccion == null)
        {
            throw new EventoException("La atracción no está asignada a este evento");
        }

        Atracciones.Remove(atraccion);
    }

    private bool YaContieneAtraccion(Guid atraccionId)
    {
        foreach(Atraccion atraccion in Atracciones)
        {
            if(atraccion.Id == atraccionId)
            {
                return true;
            }
        }

        return false;
    }

    private Atraccion? BuscarAtraccion(Guid atraccionId)
    {
        foreach(Atraccion atraccion in Atracciones)
        {
            if(atraccion.Id == atraccionId)
            {
                return atraccion;
            }
        }

        return null;
    }
}
