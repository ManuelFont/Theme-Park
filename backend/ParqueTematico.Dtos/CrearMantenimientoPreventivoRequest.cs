namespace Dtos;

public class CrearMantenimientoPreventivoRequest
{
    public Guid AtraccionId { get; set; }
    public required string Descripcion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
}
