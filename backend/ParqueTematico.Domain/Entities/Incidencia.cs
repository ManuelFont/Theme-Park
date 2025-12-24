using System.ComponentModel;
using Dominio.Exceptions;

namespace Dominio.Entities;

public enum TipoIncidencia
{
    /// <summary>
    ///     La atracción se encuentra fuera de servicio.
    /// </summary>
    [Description("Fuera de servicio")]
    FueraDeServicio,

    /// <summary>
    ///     La atracción se encuentra en mantenimiento.
    /// </summary>
    [Description("Mantenimiento")]
    Mantenimiento,

    /// <summary>
    ///     La atracción se encuentra rota.
    /// </summary>
    [Description("Rota")]
    Rota
}

public class Incidencia
{
    protected Incidencia()
    {
    }

    public Incidencia(Atraccion atraccion, TipoIncidencia tipoIncidencia, string descripcion, bool estaActiva)
    {
        if(string.IsNullOrEmpty(descripcion))
        {
            throw new IncidenciaException("La descripción no puede ser nula");
        }

        Atraccion = atraccion ?? throw new IncidenciaException("La atracción no puede ser nula");
        AtraccionId = atraccion.Id;
        TipoIncidencia = tipoIncidencia;
        Descripcion = descripcion;
        EstaActiva = estaActiva;
    }

    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AtraccionId { get; set; }
    public Atraccion Atraccion { get; private set; } = null!;
    public TipoIncidencia TipoIncidencia { get; private set; }
    public string Descripcion { get; private set; } = null!;
    public bool EstaActiva { get; private set; }

    public void Activar()
    {
        EstaActiva = true;
    }

    public void Desactivar()
    {
        EstaActiva = false;
    }
}
