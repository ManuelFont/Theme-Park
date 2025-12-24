using Dominio.Entities;

namespace ParqueTematico.BusinessLogicInterface;

public interface IHistorialPuntuacionService
{
    IList<HistorialPuntuacion> ObtenerHistorialPorVisitante(Guid visitanteId);

    void RegistrarHistorial(HistorialPuntuacion historial);
}
