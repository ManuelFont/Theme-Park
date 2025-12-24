using Dominio.Entities.Puntuacion;

namespace ParqueTematico.BusinessLogicInterface;

public interface IRankingService
{
    IPuntuacion ObtenerEstrategiaActiva();

    string ObtenerNombreEstrategiaActiva();

    bool CambiarEstrategia(string nombreEstrategia);

    IEnumerable<(string Nombre, string Descripcion)> ObtenerEstrategiasDisponibles();

    IEnumerable<(Guid VisitanteId, string NombreVisitante, int PuntosTotales)> ObtenerRankingDiario(int top = 10);
}
