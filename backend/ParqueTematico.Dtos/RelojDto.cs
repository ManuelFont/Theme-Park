namespace Dtos;

public class RelojDto(DateTime fechaHora)
{
    public DateTime FechaHora { get; set; } = fechaHora;
}
