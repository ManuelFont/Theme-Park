using Dominio.Entities;

namespace RepositoryInterfaces;

public interface IIncidenciaRepository : IBaseRepository<Incidencia>
{
    bool ExisteActiva(Guid atraccionId);
    IList<Incidencia> ObtenerActivasPorAtraccion(Guid atraccionId);
}
