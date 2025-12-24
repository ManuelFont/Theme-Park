using Dominio.Entities;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace ParqueTematico.Application.Services;

public class RelojService : IRelojService
{
    private readonly List<IRelojObserver> _observers = [];
    private readonly IRelojRepository _repository;

    public RelojService(IRelojRepository repository)
    {
        _repository = repository;
        if(_repository.ObtenerPorId(1) == null)
        {
            var relojSingleton = new Reloj();
            _repository.Agregar(relojSingleton);
        }
    }

    public Reloj ModificarFechaHora(DateTime fechaHora)
    {
        Reloj? reloj = _repository.ObtenerPorId(1);
        if(reloj == null)
        {
            throw new NullReferenceException("El Reloj no existe");
        }

        reloj.FechaHora = fechaHora;
        _repository.Actualizar(reloj);
        Notify();
        return reloj;
    }

    public DateTime ObtenerFechaHora()
    {
        Reloj? reloj = _repository.ObtenerPorId(1);
        if(reloj == null)
        {
            throw new NullReferenceException("El Reloj no existe");
        }

        return reloj.FechaHora;
    }

    public void Attach(IRelojObserver observer)
    {
        _observers.Add(observer);
    }

    public void Notify()
    {
        foreach(IRelojObserver observer in _observers)
        {
            observer.Update();
        }
    }
}
