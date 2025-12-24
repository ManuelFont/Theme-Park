using Dominio.Entities;

namespace Dtos;

public class MantenimientoPreventivoDto
{
    public Guid Id { get; set; }
    public string NombreAtraccion { get; set; } = null!;
    public Guid AtraccionId { get; set; }
    public required string Descripcion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }

    public static MantenimientoPreventivoDto FromEntity(MantenimientoPreventivo m)
    {
        return new MantenimientoPreventivoDto
        {
            Id = m.Id,
            NombreAtraccion = m.Atraccion.Nombre,
            AtraccionId = m.Atraccion.Id,
            Descripcion = m.Descripcion,
            FechaInicio = m.FechaInicio,
            FechaFin = m.FechaFin
        };
    }
}
