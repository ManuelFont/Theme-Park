using Dominio.Entities;

namespace Dtos;

public class HistorialPuntuacionDto
{
    public Guid Id { get; set; }
    public int Puntos { get; set; }
    public string Origen { get; set; } = null!;
    public string EstrategiaActiva { get; set; } = null!;
    public DateTime FechaHora { get; set; }

    public static HistorialPuntuacionDto FromEntity(HistorialPuntuacion historial)
    {
        return new HistorialPuntuacionDto
        {
            Id = historial.Id,
            Puntos = historial.Puntos,
            Origen = historial.Origen,
            EstrategiaActiva = historial.EstrategiaActiva,
            FechaHora = historial.FechaHora
        };
    }
}
