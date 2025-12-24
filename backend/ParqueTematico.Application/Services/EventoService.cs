using Dominio.Entities;
using Dominio.Exceptions;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace ParqueTematico.Application.Services;

public class EventoService(IEventoRepository repo, IAtraccionService atraccionService) : IEventoService
{
    private readonly IAtraccionService _atraccionService = atraccionService;
    private readonly IEventoRepository _repo = repo;

    public Evento CrearEvento(string nombre, DateTime fecha, TimeSpan duracion, int aforo, decimal costo)
    {
        if(string.IsNullOrWhiteSpace(nombre))
        {
            throw new EventoException("El nombre no puede estar vacío");
        }

        if(fecha < DateTime.Now)
        {
            throw new EventoException("La fecha del evento no puede ser menor a la fecha actual");
        }

        if(aforo <= 0)
        {
            throw new EventoException("El aforo debe ser mayor a 0");
        }

        if(costo < 0)
        {
            throw new EventoException("El costo no puede ser negativo");
        }

        var evento = new Evento(nombre, fecha, duracion, aforo, costo);

        _repo.Agregar(evento);

        return evento;
    }

    public void Eliminar(Guid id)
    {
        _repo.Eliminar(id);
    }

    public IEnumerable<Evento> ObtenerTodos()
    {
        return _repo.ObtenerTodos();
    }

    public Evento? ObtenerPorId(Guid id)
    {
        return _repo.ObtenerPorId(id);
    }

    public void AgregarAtraccionAEvento(Guid eventoId, Guid atraccionId)
    {
        Evento? evento = _repo.ObtenerPorId(eventoId);
        if(evento == null)
        {
            throw new EventoException("El evento no existe");
        }

        Atraccion? atraccion = _atraccionService.ObtenerPorId(atraccionId);
        if(atraccion == null)
        {
            throw new EventoException("La atracción no existe");
        }

        evento.AgregarAtraccion(atraccion);
        _repo.Actualizar(evento);
    }

    public void EliminarAtraccionDeEvento(Guid eventoId, Guid atraccionId)
    {
        Evento? evento = _repo.ObtenerPorId(eventoId);
        if(evento == null)
        {
            throw new EventoException("El evento no existe");
        }

        evento.EliminarAtraccion(atraccionId);
        _repo.Actualizar(evento);
    }
}
