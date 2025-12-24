using Dominio.Entities;

namespace Dtos;

public class CrearIncidenciaRequest
{
    public Guid AtraccionId { get; set; }
    public TipoIncidencia TipoIncidencia { get; set; }
    public required string Descripcion { get; set; }
}
