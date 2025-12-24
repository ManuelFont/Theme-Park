using RepositoryInterfaces;

namespace Infrastructure.Repositories;

public class BaseRepository<T>(ParqueDbContext context) : IBaseRepository<T>
    where T : class
{
    private readonly ParqueDbContext _context = context;

    public T Agregar(T entidad)
    {
        _context.Set<T>().Add(entidad);
        _context.SaveChanges();
        return entidad;
    }

    public IList<T> ObtenerTodos()
    {
        return _context.Set<T>().ToList();
    }

    public T? ObtenerPorId(Guid id)
    {
        return _context.Set<T>().Find(id);
    }

    public void Actualizar(T entidad)
    {
        _context.Set<T>().Update(entidad);
        _context.SaveChanges();
    }

    public void Eliminar(Guid id)
    {
        var entidad = ObtenerPorId(id);
        if(entidad != null)
        {
            _context.Set<T>().Remove(entidad);
            _context.SaveChanges();
        }
    }
}
