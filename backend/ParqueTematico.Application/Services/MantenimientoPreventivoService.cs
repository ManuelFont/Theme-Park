using Dominio.Entities;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace ParqueTematico.Application.Services;

public class MantenimientoPreventivoService : IRelojObserver, IMantenimientoPreventivoService
{
    private readonly IRelojService _reloj;
    private readonly IMantenimientoPreventivoRepository _repository;
    private readonly IAtraccionService _atraccionService;

    public MantenimientoPreventivoService(
        IMantenimientoPreventivoRepository repository,
        IRelojService reloj,
        IAtraccionService atraccionService)
    {
        _repository = repository;
        _reloj = reloj;
        _atraccionService = atraccionService;
        _reloj.Attach(this);
    }

    public MantenimientoPreventivo Crear(Guid atraccionId, string descripcion, DateTime fechaInicio, DateTime fechaFin)
    {
        var atraccion = _atraccionService.ObtenerPorId(atraccionId);
        if(atraccion == null)
        {
            throw new InvalidOperationException("La atracción no existe");
        }

        var estaActiva = DebeActivarse(fechaInicio, fechaFin);
        var mantenimiento = new MantenimientoPreventivo(atraccion, descripcion, estaActiva, fechaInicio, fechaFin);

        return _repository.Agregar(mantenimiento);
    }

    public MantenimientoPreventivo? ObtenerPorId(Guid id)
    {
        return _repository.ObtenerPorId(id);
    }

    public IEnumerable<MantenimientoPreventivo> ObtenerTodos()
    {
        return _repository.ObtenerTodos();
    }

    public void Cerrar(Guid id)
    {
        var mantenimiento = _repository.ObtenerPorId(id);
        if(mantenimiento == null)
        {
            throw new InvalidOperationException("El mantenimiento no existe");
        }

        mantenimiento.Desactivar();
        _repository.Actualizar(mantenimiento);
    }

    public void Update()
    {
        foreach(var mantenimiento in _repository.ObtenerTodos())
        {
            if(DebeActivarse(mantenimiento.FechaInicio, mantenimiento.FechaFin))
            {
                if(!mantenimiento.EstaActiva)
                {
                    mantenimiento.Activar();
                    _repository.Actualizar(mantenimiento);
                }
            }
            else
            {
                if(mantenimiento.EstaActiva)
                {
                    mantenimiento.Desactivar();
                    _repository.Actualizar(mantenimiento);
                }
            }
        }
    }

    private bool DebeActivarse(DateTime inicio, DateTime fin)
    {
        return _reloj.ObtenerFechaHora() >= inicio && _reloj.ObtenerFechaHora() <= fin;
    }
}
