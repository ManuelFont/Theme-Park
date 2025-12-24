using Dominio.Entities;
using Dominio.Exceptions;
using Microsoft.EntityFrameworkCore;
using RepositoryInterfaces;

namespace Infrastructure.Repositories;

public class EventoDbRepository(ParqueDbContext ctx) : IEventoRepository
{
    private readonly ParqueDbContext _context = ctx;

    public Evento Agregar(Evento evento)
    {
        if(_context.Eventos.AsNoTracking().Any(e => e.Id == evento.Id))
        {
            throw new EventoException("El evento ya existe en la base de datos");
        }

        foreach(var a in evento.Atracciones)
        {
            _context.Attach(a);
        }

        _context.Eventos.Add(evento);
        _context.SaveChanges();
        return evento;
    }

    public void Eliminar(Guid id)
    {
        var ev = _context.Eventos.FirstOrDefault(e => e.Id == id);
        if(ev == null)
        {
            throw new EventoException("El evento no existe");
        }

        _context.Eventos.Remove(ev);
        _context.SaveChanges();
    }

    public IList<Evento> ObtenerTodos()
    {
        return _context.Eventos
            .AsNoTracking()
            .Include(e => e.Atracciones)
            .ToList();
    }

    public Evento? ObtenerPorId(Guid id)
    {
        return _context.Eventos
            .Include(e => e.Atracciones)
            .AsNoTracking()
            .FirstOrDefault(e => e.Id == id);
    }

    public void Actualizar(Evento eventoConCambios)
    {
        var eventoPersistido = _context.Eventos
            .Include(e => e.Atracciones)
            .FirstOrDefault(e => e.Id == eventoConCambios.Id)
            ?? throw new EventoException("El evento no existe");

        _context.Entry(eventoPersistido).CurrentValues.SetValues(eventoConCambios);

        eventoPersistido.Atracciones.Clear();
        foreach(var atraccion in eventoConCambios.Atracciones)
        {
            _context.Attach(atraccion);
            eventoPersistido.Atracciones.Add(atraccion);
        }

        _context.SaveChanges();
    }
}
