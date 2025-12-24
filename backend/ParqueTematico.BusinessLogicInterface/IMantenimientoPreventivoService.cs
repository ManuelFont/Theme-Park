using Dominio.Entities;

namespace ParqueTematico.BusinessLogicInterface;

public interface IMantenimientoPreventivoService
{
    MantenimientoPreventivo Crear(Guid atraccionId, string descripcion, DateTime fechaInicio, DateTime fechaFin);

    MantenimientoPreventivo? ObtenerPorId(Guid id);

    IEnumerable<MantenimientoPreventivo> ObtenerTodos();

    void Cerrar(Guid id);
}
