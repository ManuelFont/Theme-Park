using Dominio.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryInterfaces;

namespace Infrastructure.Repositories;

public class AtraccionRepository(ParqueDbContext db)
     : BaseRepository<Atraccion>(db), IAtraccionRepository
{
    private readonly ParqueDbContext _db = db;

    public Atraccion? ObtenerPorIdConIncidencias(Guid id)
    {
        return _db.Atracciones
            .Include(a => a.Incidencias)
            .FirstOrDefault(a => a.Id == id);
    }

    public IEnumerable<Atraccion> ObtenerTodosConIncidencias()
    {
        return _db.Atracciones
            .Include(a => a.Incidencias)
            .ToList();
    }
}
