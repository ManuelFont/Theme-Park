using Dominio.Entities;

namespace ParqueTematico.BusinessLogicInterface;

public interface IIncidenciaService
{
    Incidencia Crear(Guid atraccionId, TipoIncidencia tipo, string descripcion);

    void Cerrar(Guid incidenciaId);

    bool ExisteActiva(Guid atraccionId);

    IList<Incidencia> ObtenerActivasPorAtraccion(Guid atraccionId);
}
