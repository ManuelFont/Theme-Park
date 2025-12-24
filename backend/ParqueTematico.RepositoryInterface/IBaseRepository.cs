namespace RepositoryInterfaces;

public interface IBaseRepository<T>
{
    T Agregar(T entidad);
    IList<T> ObtenerTodos();
    T? ObtenerPorId(Guid id);
    void Actualizar(T entidad);
    void Eliminar(Guid id);
}
