using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Dominio.Exceptions;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.ExcepcionesRepo;
using Infrastructure.Repositories;

namespace Test.Repositories;

[TestClass]
public class TicketDbRepositoryTest
{
    private ParqueDbContext _context = null!;
    private TicketDbRepository _repository = null!;
    private UsuarioDbRepository _usuarioRepo = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _repository = new TicketDbRepository(_context);
        _usuarioRepo = new UsuarioDbRepository(_context);
    }

    private static Visitante CrearVisitanteValido()
    {
        return new Visitante(
            "Juan",
            "Latorre",
            "juan.latorre@test.com",
            "ClaveContra1!",
            new DateTime(1995, 5, 10),
            NivelMembresia.Estandar);
    }

    private static Evento CrearEventoValido()
    {
        return new Evento(
            "Concierto",
            DateTime.Now.AddDays(7),
            new TimeSpan(20, 0, 0),
            200,
            150m);
    }

    [TestMethod]
    public void Agregar_TicketGeneral_guarda_correctamente()
    {
        Visitante visitante = CrearVisitanteValido();
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);

        Ticket creado = _repository.Agregar(ticket);
        _context.SaveChanges();

        Assert.IsTrue(creado.Id != Guid.Empty);
        Ticket enDb = _context.Tickets.Single(x => x.Id == creado.Id);
        Assert.AreEqual(TipoEntrada.General, enDb.TipoEntrada);
        Assert.IsNotNull(enDb.Visitante);
        Assert.IsNull(enDb.EventoAsociado);
    }

    [TestMethod]
    public void Agregar_TicketEventoEspecial_con_evento_guarda_correctamente()
    {
        Visitante visitante = CrearVisitanteValido();
        Evento evento = CrearEventoValido();
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(3), TipoEntrada.EventoEspecial, evento);

        Ticket creado = _repository.Agregar(ticket);
        _context.SaveChanges();

        Assert.IsTrue(creado.Id != Guid.Empty);
        Ticket enDb = _context.Tickets.Single(x => x.Id == creado.Id);
        Assert.AreEqual(TipoEntrada.EventoEspecial, enDb.TipoEntrada);
        Assert.IsNotNull(enDb.EventoAsociado);
        Assert.AreEqual(evento.Id, enDb.EventoAsociado!.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(TicketException))]
    public void Agregar_evento_especial_sin_evento_debe_lanzar()
    {
        Visitante visitante = CrearVisitanteValido();
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(2), TipoEntrada.EventoEspecial, null);
        _repository.Agregar(ticket);
    }

    [TestMethod]
    public void Agregar_incrementa_cantidad()
    {
        var antes = _context.Tickets.Count();
        Visitante visitante = CrearVisitanteValido();
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(2), TipoEntrada.General, null);

        _repository.Agregar(ticket);
        _context.SaveChanges();

        var despues = _context.Tickets.Count();
        Assert.AreEqual(antes + 1, despues);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public void ObtenerPorId_existente_devuelve_ticket()
    {
        Visitante visitante = CrearVisitanteValido();
        _usuarioRepo.Agregar(visitante);
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);

        _repository.Agregar(ticket);
        _context.SaveChanges();

        Ticket? resultado = _repository.ObtenerPorId(ticket.Id);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(ticket.Id, resultado!.Id);
        Assert.AreEqual(TipoEntrada.General, resultado.TipoEntrada);
    }

    [TestMethod]
    public void ObtenerPorId_inexistente_devuelve_null()
    {
        Ticket? resultado = _repository.ObtenerPorId(Guid.NewGuid());
        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void Eliminar_existente_quita_ticket()
    {
        Visitante visitante = CrearVisitanteValido();
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);

        _repository.Agregar(ticket);
        _context.SaveChanges();

        _repository.Eliminar(ticket.Id);

        Ticket? enDb = _context.Tickets.Find(ticket.Id);
        Assert.IsNull(enDb);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionRepositorioTicket))]
    public void Eliminar_inexistente_lanza_excepcion()
    {
        _repository.Eliminar(Guid.NewGuid());
    }

    [TestMethod]
    public void Eliminar_disminuye_cantidad()
    {
        Visitante visitante = CrearVisitanteValido();
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(2), TipoEntrada.General, null);

        _repository.Agregar(ticket);
        _context.SaveChanges();

        var antes = _context.Tickets.Count();
        _repository.Eliminar(ticket.Id);
        var despues = _context.Tickets.Count();

        Assert.AreEqual(antes - 1, despues);
    }

    [TestMethod]
    public void Actualizar_fecha_valida_modifica_ticket()
    {
        Visitante visitante = CrearVisitanteValido();
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(2), TipoEntrada.General, null);
        _repository.Agregar(ticket);
        _context.SaveChanges();

        var modificado =
            new Ticket(visitante, DateTime.Now.AddDays(10), ticket.TipoEntrada, ticket.EventoAsociado)
            {
                Id = ticket.Id
            };

        _repository.Actualizar(modificado);

        Ticket enDb = _context.Tickets.Find(ticket.Id)!;
        Assert.AreEqual(modificado.FechaVisita.Date, enDb.FechaVisita.Date);
    }

    [TestMethod]
    [ExpectedException(typeof(TicketException))]
    public void Actualizar_fecha_pasada_lanza_excepcion()
    {
        Visitante visitante = CrearVisitanteValido();
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(2), TipoEntrada.General, null);
        _repository.Agregar(ticket);
        _context.SaveChanges();

        var modificado =
            new Ticket(visitante, DateTime.Now.AddDays(-2), ticket.TipoEntrada, ticket.EventoAsociado)
            {
                Id = ticket.Id
            };

        _repository.Actualizar(modificado);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionRepositorioTicket))]
    public void Actualizar_inexistente_lanza_excepcion()
    {
        Visitante visitante = CrearVisitanteValido();
        var modificado =
            new Ticket(visitante, DateTime.Now.AddDays(5), TipoEntrada.General, null) { Id = Guid.NewGuid() };

        _repository.Actualizar(modificado);
    }

    [TestMethod]
    public void ObtenerTodos_devuelve_lista_con_tickets_existentes()
    {
        Visitante visitante = CrearVisitanteValido();
        _usuarioRepo.Agregar(visitante);
        var t1 = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);
        var t2 = new Ticket(visitante, DateTime.Now.AddDays(2), TipoEntrada.General, null);
        _repository.Agregar(t1);
        _repository.Agregar(t2);
        _context.SaveChanges();

        IList<Ticket> lista = _repository.ObtenerTodos();
        Assert.AreEqual(2, lista.Count);
        Assert.IsTrue(lista.Any(x => x.Id == t1.Id));
        Assert.IsTrue(lista.Any(x => x.Id == t2.Id));
    }

    [TestMethod]
    public void ObtenerTodos_sin_tickets_devuelve_lista_vacia()
    {
        IList<Ticket> lista = _repository.ObtenerTodos();
        Assert.AreEqual(0, lista.Count);
    }
}
