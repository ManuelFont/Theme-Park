using Dominio.Entities;
using RepositoryInterfaces;

namespace Infrastructure.Repositories;

public class RelojDbRepository(ParqueDbContext context) : IRelojRepository
{
    private readonly ParqueDbContext _context = context;

    public Reloj? ObtenerPorId(int id) => _context.Relojes.Find(id);

    public Reloj Agregar(Reloj reloj)
    {
        _context.Relojes.Add(reloj);
        _context.SaveChanges();
        return reloj;
    }

    public void Eliminar(int id)
    {
        var reloj = _context.Relojes.Find(id)
            ?? throw new InvalidOperationException("El reloj no existe");
        _context.Relojes.Remove(reloj);
        _context.SaveChanges();
    }

    public void Actualizar(Reloj reloj)
    {
        _context.Relojes.Update(reloj);
        _context.SaveChanges();
    }
}
