using Dominio.Entities;

namespace Dtos;

public class IncidenciaDto
{
    public Guid Id { get; set; }

    public Guid AtraccionId { get; set; }
    public TipoIncidencia TipoIncidencia { get; set; }
    public string Descripcion { get; set; } = null!;
    public bool EstaActiva { get; set; }

    public static IncidenciaDto FromEntity(Incidencia incidencia)
    {
        return new IncidenciaDto
        {
            Id = incidencia.Id,
            AtraccionId = incidencia.AtraccionId,
            TipoIncidencia = incidencia.TipoIncidencia,
            Descripcion = incidencia.Descripcion,
            EstaActiva = incidencia.EstaActiva
        };
    }

    public Incidencia ToEntity(Atraccion atraccion)
    {
        return new Incidencia(atraccion, TipoIncidencia, Descripcion, EstaActiva)
        {
            Id = Id
        };
    }
}
