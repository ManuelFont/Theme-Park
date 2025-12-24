using Dominio.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryInterfaces;

namespace Infrastructure.Repositories;

public class HistorialPuntuacionDbRepository(ParqueDbContext context) : BaseRepository<HistorialPuntuacion>(context), IHistorialPuntuacionRepository
{
    private readonly ParqueDbContext _context = context;
    public IList<HistorialPuntuacion> ObtenerPorVisitante(Guid visitanteId)
    {
        return _context.HistorialesPuntuacion
            .AsNoTracking()
            .Include(h => h.Visitante)
            .Where(h => EF.Property<Guid>(h, "VisitanteId") == visitanteId)
            .OrderByDescending(h => h.FechaHora)
            .ToList();
    }
}
