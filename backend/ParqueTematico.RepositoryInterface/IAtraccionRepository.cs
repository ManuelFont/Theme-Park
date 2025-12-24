using Dominio.Entities;

namespace RepositoryInterfaces;

public interface IAtraccionRepository : IBaseRepository<Atraccion>
{
    Atraccion? ObtenerPorIdConIncidencias(Guid id);
    IEnumerable<Atraccion> ObtenerTodosConIncidencias();
}
