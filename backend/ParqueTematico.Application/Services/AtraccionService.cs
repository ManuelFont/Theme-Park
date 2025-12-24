using Dominio.Entities;
using Dtos;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace ParqueTematico.Application.Services;

public class AtraccionService(IBaseRepository<Atraccion> repo) : IAtraccionService
{
    private readonly IBaseRepository<Atraccion> _repo = repo;

    public void Crear(string nombre, TipoAtraccion tipo, int edadMinima, int capacidadMaxima, string descripcion, bool disponible)
    {
        var atraccion = new Atraccion(nombre, tipo, edadMinima, capacidadMaxima, descripcion, disponible);
        _repo.Agregar(atraccion);
    }

    public void Actualizar(AtraccionDto atraccionDto)
    {
        Atraccion? atraccionDb = ObtenerPorId(atraccionDto.Id);
        if(atraccionDb == null)
        {
            throw new InvalidOperationException("No existe atraccion con ese Id");
        }

        atraccionDb.Actualizar(atraccionDto.Nombre, atraccionDto.Tipo, atraccionDto.EdadMinima,
            atraccionDto.CapacidadMaxima, atraccionDto.Descripcion, atraccionDto.Disponible);
        _repo.Actualizar(atraccionDb);
    }

    public void Eliminar(Guid id)
    {
        _repo.Eliminar(id);
    }

    public Atraccion? ObtenerPorId(Guid id)
    {
        return _repo.ObtenerPorId(id);
    }

    public IEnumerable<Atraccion> ObtenerTodos()
    {
        return _repo.ObtenerTodos();
    }
}
