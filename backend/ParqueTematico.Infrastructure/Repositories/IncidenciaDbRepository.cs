using Dominio.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryInterfaces;

namespace Infrastructure.Repositories;

public class IncidenciaDbRepository(ParqueDbContext context) : BaseRepository<Incidencia>(context), IIncidenciaRepository
{
    private readonly ParqueDbContext _context = context;

    public bool ExisteActiva(Guid atraccionId)
    {
        return _context.Incidencias
            .AsNoTracking()
            .Any(i => EF.Property<Guid>(i, "AtraccionId") == atraccionId && i.EstaActiva);
    }

    public IList<Incidencia> ObtenerActivasPorAtraccion(Guid atraccionId)
    {
        return _context.Incidencias
            .AsNoTracking()
            .Include(i => i.Atraccion)
            .Where(i => EF.Property<Guid>(i, "AtraccionId") == atraccionId && i.EstaActiva)
            .ToList();
    }
}
