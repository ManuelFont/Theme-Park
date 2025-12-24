using Dominio.Entities;

namespace RepositoryInterfaces;

public interface ICanjeRepository : IBaseRepository<Canje>
{
    IList<Canje> ObtenerPorVisitante(Guid visitanteId);
}
