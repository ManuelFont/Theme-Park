using Dominio.Entities.Puntuacion;

namespace ParqueTematico.BusinessLogicInterface;

public interface IEstrategiaService
{
    IPuntuacion ObtenerEstrategiaActiva();

    bool CambiarEstrategia(string nombreEstrategia);

    IEnumerable<(string Nombre, string Descripcion)> ObtenerEstrategiasDisponibles();
}
