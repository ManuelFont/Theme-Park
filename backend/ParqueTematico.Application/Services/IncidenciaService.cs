using Dominio.Entities;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace ParqueTematico.Application.Services;

public class IncidenciaService(
    IIncidenciaRepository incidenciaRepo,
    IAtraccionService atraccionService) : IIncidenciaService
{
    private readonly IIncidenciaRepository _incidenciaRepo = incidenciaRepo;
    private readonly IAtraccionService _atraccionService = atraccionService;

    public Incidencia Crear(Guid atraccionId, TipoIncidencia tipo, string descripcion)
    {
        var atraccion = _atraccionService.ObtenerPorId(atraccionId);
        if(atraccion == null)
        {
            throw new InvalidOperationException("La atracción no existe");
        }

        var incidencia = new Incidencia(atraccion, tipo, descripcion, estaActiva: true);
        return _incidenciaRepo.Agregar(incidencia);
    }

    public void Cerrar(Guid incidenciaId)
    {
        var incidencia = _incidenciaRepo.ObtenerPorId(incidenciaId);
        if(incidencia == null)
        {
            throw new InvalidOperationException("La incidencia no existe");
        }

        incidencia.Desactivar();
        _incidenciaRepo.Actualizar(incidencia);
    }

    public bool ExisteActiva(Guid atraccionId)
    {
        return _incidenciaRepo.ExisteActiva(atraccionId);
    }

    public IList<Incidencia> ObtenerActivasPorAtraccion(Guid atraccionId)
    {
        return _incidenciaRepo.ObtenerActivasPorAtraccion(atraccionId);
    }
}
