using Dominio.Entities;
using Dtos;

public class EventoDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!;
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public int Aforo { get; set; }
    public decimal CostoAdicional { get; set; }
    public List<AtraccionDto> Atracciones { get; set; } = [];

    public static EventoDto FromEntity(Evento evento)
    {
        return new EventoDto
        {
            Id = evento.Id,
            Nombre = evento.Nombre,
            Fecha = evento.Fecha,
            Hora = evento.Hora,
            Aforo = evento.Aforo,
            CostoAdicional = evento.CostoAdicional,
            Atracciones = evento.Atracciones.Select(AtraccionDto.FromEntity).ToList()
        };
    }
}
