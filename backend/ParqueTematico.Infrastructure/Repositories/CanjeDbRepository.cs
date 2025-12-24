using Dominio.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryInterfaces;

namespace Infrastructure.Repositories;

public class CanjeDbRepository(ParqueDbContext context) : BaseRepository<Canje>(context), ICanjeRepository
{
    private readonly ParqueDbContext _context = context;

    public IList<Canje> ObtenerPorVisitante(Guid visitanteId)
    {
        return _context.Canjes
            .AsNoTracking()
            .Include(c => c.Usuario)
            .Include(c => c.Recompensa)
            .Where(c => c.Usuario.Id == visitanteId)
            .OrderByDescending(c => c.FechaCanje)
            .ToList();
    }
}
