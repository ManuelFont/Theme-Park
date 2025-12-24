using Dominio.Entities.Usuarios;

namespace Dtos;

public class RecompensaCrearDto
{
    public required string Nombre { get; set; }
    public required string Descripcion { get; set; }
    public int Costo { get; set; }
    public int CantidadDisponible { get; set; }
    public NivelMembresia? NivelRequerido { get; set; }
}
