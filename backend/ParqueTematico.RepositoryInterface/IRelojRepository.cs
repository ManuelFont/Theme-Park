using Dominio.Entities;

namespace RepositoryInterfaces;

public interface IRelojRepository
{
    Reloj? ObtenerPorId(int id);
    Reloj Agregar(Reloj reloj);
    void Eliminar(int id);
    void Actualizar(Reloj reloj);
}
