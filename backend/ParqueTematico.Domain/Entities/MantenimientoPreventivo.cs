using Dominio.Exceptions;

namespace Dominio.Entities;

public class MantenimientoPreventivo : Incidencia
{
    private MantenimientoPreventivo()
    {
    }

    public MantenimientoPreventivo(Atraccion atraccion, string descripcion, bool estaActiva, DateTime fechaInicio,
        DateTime fechaFin)
        : base(atraccion, TipoIncidencia.Mantenimiento, descripcion, estaActiva)
    {
        if(fechaFin <= fechaInicio)
        {
            throw new IncidenciaException("La fecha fin no puede ser menor ni igual a la fecha inicio");
        }

        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
    }

    public DateTime FechaInicio { get; }
    public DateTime FechaFin { get; }
}
