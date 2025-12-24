using Dominio.Entities;

namespace RepositoryInterfaces;

public interface IHistorialPuntuacionRepository : IBaseRepository<HistorialPuntuacion>
{
    IList<HistorialPuntuacion> ObtenerPorVisitante(Guid visitanteId);
}
