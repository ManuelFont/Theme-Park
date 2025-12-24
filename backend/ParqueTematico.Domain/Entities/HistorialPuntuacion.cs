using Dominio.Entities.Usuarios;
using Dominio.Exceptions;

namespace Dominio.Entities;

public class HistorialPuntuacion
{
    private HistorialPuntuacion()
    {
    }

    public HistorialPuntuacion(Visitante visitante, int puntos, string origen, string estrategiaActiva,
        DateTime fechaHora)
    {
        if(visitante == null)
        {
            throw new HistorialPuntuacionException("El visitante no puede ser nulo");
        }

        if(string.IsNullOrWhiteSpace(origen))
        {
            throw new HistorialPuntuacionException("El origen no puede ser nulo o vacío");
        }

        if(string.IsNullOrWhiteSpace(estrategiaActiva))
        {
            throw new HistorialPuntuacionException("La estrategia activa no puede ser nula o vacía");
        }

        Visitante = visitante;
        Puntos = puntos;
        Origen = origen;
        EstrategiaActiva = estrategiaActiva;
        FechaHora = fechaHora;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Visitante Visitante { get; private set; } = null!;
    public int Puntos { get; private set; }
    public string Origen { get; private set; } = null!;
    public string EstrategiaActiva { get; private set; } = null!;
    public DateTime FechaHora { get; private set; }
}
