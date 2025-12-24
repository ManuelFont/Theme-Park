using Dominio.Entities;
using Dtos;

namespace ParqueTematico.BusinessLogicInterface;

public interface IAtraccionService
{
    void Crear(string nombre, TipoAtraccion tipo, int edadMinima, int capacidadMaxima, string descripcion, bool disponible);

    void Actualizar(AtraccionDto atraccionDto);

    void Eliminar(Guid id);

    Atraccion? ObtenerPorId(Guid id);

    IEnumerable<Atraccion> ObtenerTodos();
}
