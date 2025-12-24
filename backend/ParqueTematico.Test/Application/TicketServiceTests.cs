using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Dominio.Exceptions;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using ParqueTematico.Application.Services;

namespace Test.Application;

[TestClass]
public class TicketServiceTests
{
    private ParqueDbContext _context = null!;
    private TicketDbRepository _ticketRepo = null!;
    private UsuarioDbRepository _usuarioRepo = null!;
    private EventoDbRepository _eventoRepo = null!;
    private RelojDbRepository _relojRepo = null!;
    private EventoService _eventoService = null!;
    private RelojService _relojService = null!;
    private TicketService _service = null!;

    [TestInitialize]
    public void SetUp()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _ticketRepo = new TicketDbRepository(_context);
        _usuarioRepo = new UsuarioDbRepository(_context);
        _eventoRepo = new EventoDbRepository(_context);
        _relojRepo = new RelojDbRepository(_context);
        var atraccionRepo = new AtraccionRepository(_context);
        var atraccionService = new AtraccionService(atraccionRepo);
        _eventoService = new EventoService(_eventoRepo, atraccionService);
        _relojService = new RelojService(_relojRepo);
        _service = new TicketService(_ticketRepo, _usuarioRepo, _eventoService, _relojService);
    }

    [TestMethod]
    public void ComprarTicket_TicketGeneralValido_DeberiaCrearTicket()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1), NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var fechaVisita = DateTime.Now.AddDays(10);

        var ticketId = _service.ComprarTicket(visitante.Id, fechaVisita, TipoEntrada.General, null);

        var ticket = _ticketRepo.ObtenerPorId(ticketId);
        Assert.IsNotNull(ticket);
        Assert.AreEqual(visitante.Id, ticket.Visitante.Id);
        Assert.AreEqual(fechaVisita.Date, ticket.FechaVisita.Date);
        Assert.AreEqual(TipoEntrada.General, ticket.TipoEntrada);
        Assert.IsNull(ticket.EventoAsociado);
    }

    [TestMethod]
    public void ObtenerPorId_TicketExistente_DeberiaRetornarTicket()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1), NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var fechaVisita = DateTime.Now.AddDays(10);
        var ticketId = _service.ComprarTicket(visitante.Id, fechaVisita, TipoEntrada.General, null);

        var ticket = _service.ObtenerPorId(ticketId);

        Assert.IsNotNull(ticket);
        Assert.AreEqual(ticketId, ticket.Id);
    }

    [TestMethod]
    public void ObtenerPorId_TicketInexistente_DeberiaRetornarNull()
    {
        var ticketId = Guid.NewGuid();

        var ticket = _service.ObtenerPorId(ticketId);

        Assert.IsNull(ticket);
    }

    [TestMethod]
    public void ObtenerTicketsPorVisitante_VisitanteConTickets_DeberiaRetornarTodosLosTickets()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1), NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var fechaVisita1 = DateTime.Now.AddDays(10);
        var fechaVisita2 = DateTime.Now.AddDays(20);
        _service.ComprarTicket(visitante.Id, fechaVisita1, TipoEntrada.General, null);
        _service.ComprarTicket(visitante.Id, fechaVisita2, TipoEntrada.General, null);

        var tickets = _service.ObtenerTicketsPorVisitante(visitante.Id);

        Assert.AreEqual(2, tickets.Count());
    }

    [TestMethod]
    public void ObtenerTicketsPorVisitante_VisitanteSinTickets_DeberiaRetornarListaVacia()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1), NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);

        var tickets = _service.ObtenerTicketsPorVisitante(visitante.Id);

        Assert.AreEqual(0, tickets.Count());
    }

    [TestMethod]
    public void ComprarTicket_TicketEventoEspecialValido_DeberiaCrearTicket()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1), NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var evento = new Evento("Noche de Dinosaurios", DateTime.Now.AddDays(30), new TimeSpan(20, 0, 0), 100, 500);
        _eventoRepo.Agregar(evento);
        var eventoId = evento.Id;
        var fechaVisita = DateTime.Now.AddDays(30);
        _context.ChangeTracker.Clear();

        var ticketId = _service.ComprarTicket(visitante.Id, fechaVisita, TipoEntrada.EventoEspecial, eventoId);

        var ticket = _ticketRepo.ObtenerPorId(ticketId);
        Assert.IsNotNull(ticket);
        Assert.AreEqual(TipoEntrada.EventoEspecial, ticket.TipoEntrada);
        Assert.IsNotNull(ticket.EventoAsociado);
        Assert.AreEqual(eventoId, ticket.EventoAsociado.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(TicketException))]
    public void ComprarTicket_UsuarioNoEsVisitante_DeberiaLanzarExcepcion()
    {
        var admin = new Administrador("Admin", "Test", "admin@test.com", "Pass1234!");
        _usuarioRepo.Agregar(admin);
        var fechaVisita = DateTime.Now.AddDays(10);

        _service.ComprarTicket(admin.Id, fechaVisita, TipoEntrada.General, null);
    }

    [TestMethod]
    [ExpectedException(typeof(TicketException))]
    public void ComprarTicket_EventoEspecialSinEventoId_DeberiaLanzarExcepcion()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1), NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var fechaVisita = DateTime.Now.AddDays(10);

        _service.ComprarTicket(visitante.Id, fechaVisita, TipoEntrada.EventoEspecial, null);
    }

    [TestMethod]
    [ExpectedException(typeof(TicketException))]
    public void ComprarTicket_GeneralConEventoId_DeberiaLanzarExcepcion()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1), NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var evento = new Evento("Evento Test", DateTime.Now.AddDays(30), new TimeSpan(20, 0, 0), 100, 500);
        _eventoRepo.Agregar(evento);
        var fechaVisita = DateTime.Now.AddDays(10);

        _service.ComprarTicket(visitante.Id, fechaVisita, TipoEntrada.General, evento.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(TicketException))]
    public void ComprarTicket_EventoInexistente_DeberiaLanzarExcepcion()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1), NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var fechaVisita = DateTime.Now.AddDays(10);

        _service.ComprarTicket(visitante.Id, fechaVisita, TipoEntrada.EventoEspecial, Guid.NewGuid());
    }

    [TestMethod]
    [ExpectedException(typeof(TicketException))]
    public void ComprarTicket_AforoCompleto_DeberiaLanzarExcepcion()
    {
        var visitante1 = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1), NivelMembresia.Estandar);
        var visitante2 = new Visitante("Maria", "Lopez", "maria@test.com", "Pass1234!", new DateTime(2000, 1, 1), NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante1);
        _usuarioRepo.Agregar(visitante2);
        var evento = new Evento("Evento Lleno", DateTime.Now.AddDays(30), new TimeSpan(20, 0, 0), 1, 500);
        _eventoRepo.Agregar(evento);
        var eventoId = evento.Id;
        var fechaVisita = DateTime.Now.AddDays(30);
        _context.ChangeTracker.Clear();

        _service.ComprarTicket(visitante1.Id, fechaVisita, TipoEntrada.EventoEspecial, eventoId);
        _service.ComprarTicket(visitante2.Id, fechaVisita, TipoEntrada.EventoEspecial, eventoId);
    }
}
