namespace Dtos;

public class CrearEventoRequest
{
    public required string Nombre { get; set; }
    public required DateTime Fecha { get; set; }
    public required TimeSpan Hora { get; set; }
    public required int Aforo { get; set; }
    public required decimal CostoAdicional { get; set; }
}
