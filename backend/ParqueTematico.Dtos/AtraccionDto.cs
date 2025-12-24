using Dominio.Entities;

namespace Dtos;

public class AtraccionDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!;
    public TipoAtraccion Tipo { get; set; }
    public int EdadMinima { get; set; }
    public int CapacidadMaxima { get; set; }
    public string Descripcion { get; set; } = null!;
    public bool Disponible { get; set; }
    public List<IncidenciaDto> Incidencias { get; set; } = [];

    public static AtraccionDto FromEntity(Atraccion atraccion)
    {
        return new AtraccionDto
        {
            Id = atraccion.Id,
            Nombre = atraccion.Nombre,
            Tipo = atraccion.Tipo,
            EdadMinima = atraccion.EdadMinima,
            CapacidadMaxima = atraccion.CapacidadMaxima,
            Descripcion = atraccion.Descripcion,
            Disponible = atraccion.Disponible,
            Incidencias = atraccion.Incidencias
                .Select(IncidenciaDto.FromEntity)
                .ToList()
        };
    }

    public Atraccion ToEntity()
    {
        return new Atraccion(Nombre, Tipo, EdadMinima, CapacidadMaxima, Descripcion, Disponible) { Id = Id };
    }
}
