using Dominio.Entities;
using Infrastructure.ExcepcionesRepo;
using Microsoft.EntityFrameworkCore;
using RepositoryInterfaces;

namespace Infrastructure.Repositories;

public class TicketDbRepository(ParqueDbContext ctx) : ITicketRepository
{
    private readonly ParqueDbContext _ctx = ctx;

    public Ticket Agregar(Ticket entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad);

        _ctx.Attach(entidad.Visitante);

        if(entidad.EventoAsociado != null)
        {
            _ctx.Attach(entidad.EventoAsociado);
        }

        _ctx.Tickets.Add(entidad);
        _ctx.SaveChanges();
        return entidad;
    }

    public Ticket? ObtenerPorId(Guid id)
    {
        return _ctx.Tickets
            .Include(t => t.Visitante)
            .Include(t => t.EventoAsociado)
            .FirstOrDefault(t => t.Id == id);
    }

    public void Eliminar(Guid id)
    {
        var ticket = _ctx.Tickets.FirstOrDefault(t => t.Id == id);
        if(ticket == null)
        {
            throw new ExcepcionRepositorioTicket("No se encontró un ticket con ese id");
        }

        _ctx.Tickets.Remove(ticket);
        _ctx.SaveChanges();
    }

    public void Actualizar(Ticket entidad)
    {
        ArgumentNullException.ThrowIfNull(entidad);

        var original = _ctx.Tickets.FirstOrDefault(t => t.Id == entidad.Id);
        if(original == null)
        {
            throw new ExcepcionRepositorioTicket("No se encontró un ticket con ese id");
        }

        original.CambiarFechaVisita(entidad.FechaVisita);

        _ctx.SaveChanges();
    }

    public IList<Ticket> ObtenerTodos()
    {
        return _ctx.Tickets
            .Include(t => t.Visitante)
            .Include(t => t.EventoAsociado)
            .AsNoTracking()
            .ToList();
    }
}
