namespace Dominio.Entities.Puntuacion;

public class Ranking(IPuntuacion estrategiaActiva)
{
    public IPuntuacion EstrategiaActiva { get; set; } = estrategiaActiva;
}
