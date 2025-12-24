namespace Dtos;

public class ReporteUsoAtraccionDto
{
    public Guid AtraccionId { get; set; }
    public string NombreAtraccion { get; set; } = string.Empty;
    public int CantidadVisitas { get; set; }
}
