using Dominio.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryInterfaces;

namespace Infrastructure.Repositories;

public class AccesoAtraccionDbRepository(ParqueDbContext context) : BaseRepository<AccesoAtraccion>(context), IAccesoAtraccionRepository
{
    private readonly ParqueDbContext _context = context;

    public IList<AccesoAtraccion> ObtenerAccesosSinEgresoPorAtraccion(Guid atraccionId)
    {
        return _context.AccesosAtraccion
            .AsNoTracking()
            .Where(a => EF.Property<Guid>(a, "AtraccionId") == atraccionId && a.FechaHoraEgreso == null)
            .ToList();
    }

    public IList<AccesoAtraccion> ObtenerAccesosPorVisitanteYFecha(Guid visitanteId, DateTime fecha)
    {
        return _context.AccesosAtraccion
            .AsNoTracking()
            .Include(a => a.Visitante)
            .Include(a => a.Atraccion)
            .Include(a => a.Ticket)
            .Where(a => EF.Property<Guid>(a, "VisitanteId") == visitanteId
                        && a.FechaHoraIngreso.Date == fecha.Date)
            .ToList();
    }

    public IList<AccesoAtraccion> ObtenerAccesosEntreFechas(DateTime fechaInicio, DateTime fechaFin)
    {
        return _context.AccesosAtraccion
            .AsNoTracking()
            .Include(a => a.Visitante)
            .Include(a => a.Atraccion)
            .Include(a => a.Ticket)
            .Where(a => a.FechaHoraIngreso >= fechaInicio && a.FechaHoraIngreso <= fechaFin)
            .ToList();
    }
}
