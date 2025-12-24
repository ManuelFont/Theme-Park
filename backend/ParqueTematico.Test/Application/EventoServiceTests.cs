using Dominio.Entities;
using Dominio.Exceptions;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using ParqueTematico.Application.Services;
using RepositoryInterfaces;

namespace Test.Application;

[TestClass]
public class EventoServiceTests
{
    private IAtraccionRepository _atraccionRepo = null!;
    private AtraccionService _atraccionService = null!;
    private ParqueDbContext _ctx = null!;
    private EventoDbRepository _repo = null!;
    private EventoService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _ctx = SqlContextFactory.CreateMemoryContext();
        _repo = new EventoDbRepository(_ctx);
        _atraccionRepo = new AtraccionRepository(_ctx);
        _atraccionService = new AtraccionService(_atraccionRepo);
        _service = new EventoService(_repo, _atraccionService);
    }

    [TestMethod]
    public void CrearEvento_DeberiaCrearlo()
    {
        _service!.CrearEvento("Evento", DateTime.Today.AddDays(1), new TimeSpan(1, 0, 0), 50, 100);
        IEnumerable<Evento> eventos = _service.ObtenerTodos();
        Assert.AreEqual(1, eventos.Count());
    }

    [TestMethod]
    public void EliminarEvento_DeberiaEliminarlo()
    {
        _service!.CrearEvento("Evento", DateTime.Today.AddDays(1), new TimeSpan(1, 0, 0), 50, 100);
        Evento evento = _service.ObtenerTodos().First();
        _service.Eliminar(evento.Id);
        IEnumerable<Evento> eventos = _service.ObtenerTodos();
        Assert.AreEqual(0, eventos.Count());
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void EliminarEventoQueNoExiste_DeberiaTirarExcepcion()
    {
        _service!.CrearEvento("Evento", DateTime.Today.AddDays(1), new TimeSpan(1, 0, 0), 50, 100);
        Evento evento = _service.ObtenerTodos().First();
        _service.Eliminar(evento.Id);
        _service.Eliminar(evento.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void CrearEvento_NombreVacio_DeberiaLanzarExcepcion()
    {
        _service!.CrearEvento(string.Empty, DateTime.Now.AddDays(1), new TimeSpan(2, 0, 0), 100, 50);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void CrearEvento_FechaPasada_DeberiaLanzarExcepcion()
    {
        _service!.CrearEvento("Evento", DateTime.Now.AddDays(-1), new TimeSpan(2, 0, 0), 100, 50);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void CrearEvento_AforoInvalido_DeberiaLanzarExcepcion()
    {
        _service!.CrearEvento("Evento", DateTime.Now.AddDays(1), new TimeSpan(2, 0, 0), 0, 50);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void CrearEvento_CostoInvalido_DeberiaLanzarExcepcion()
    {
        _service!.CrearEvento("Evento", DateTime.Now.AddDays(1), new TimeSpan(2, 0, 0), 100, -1);
    }

    [TestMethod]
    public void AgregarAtraccionAEvento_DeberiaAgregarla()
    {
        _service!.CrearEvento("Evento", DateTime.Today.AddDays(1), new TimeSpan(1, 0, 0), 50, 100);
        Evento evento = _service.ObtenerTodos().First();

        _atraccionService.Crear("Atraccion", TipoAtraccion.MontañaRusa, 10, 50, "Descripcion", true);
        Atraccion atraccion = _atraccionService.ObtenerTodos().First();

        _service.AgregarAtraccionAEvento(evento.Id, atraccion.Id);

        Evento? eventoActualizado = _service.ObtenerPorId(evento.Id);
        Assert.AreEqual(1, eventoActualizado!.Atracciones.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void AgregarAtraccionAEvento_EventoNoExiste_DeberiaTirarExcepcion()
    {
        _atraccionService.Crear("Atraccion", TipoAtraccion.MontañaRusa, 10, 50, "Descripcion", true);
        Atraccion atraccion = _atraccionService.ObtenerTodos().First();

        _service.AgregarAtraccionAEvento(Guid.NewGuid(), atraccion.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void AgregarAtraccionAEvento_AtraccionNoExiste_DeberiaTirarExcepcion()
    {
        _service!.CrearEvento("Evento", DateTime.Today.AddDays(1), new TimeSpan(1, 0, 0), 50, 100);
        Evento evento = _service.ObtenerTodos().First();

        _service.AgregarAtraccionAEvento(evento.Id, Guid.NewGuid());
    }

    [TestMethod]
    public void EliminarAtraccionDeEvento_DeberiaEliminarla()
    {
        _service!.CrearEvento("Evento", DateTime.Today.AddDays(1), new TimeSpan(1, 0, 0), 50, 100);
        Evento evento = _service.ObtenerTodos().First();

        _atraccionService.Crear("Atraccion", TipoAtraccion.MontañaRusa, 10, 50, "Descripcion", true);
        Atraccion atraccion = _atraccionService.ObtenerTodos().First();

        _service.AgregarAtraccionAEvento(evento.Id, atraccion.Id);
        _service.EliminarAtraccionDeEvento(evento.Id, atraccion.Id);

        Evento? eventoActualizado = _service.ObtenerPorId(evento.Id);
        Assert.AreEqual(0, eventoActualizado!.Atracciones.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void EliminarAtraccionDeEvento_EventoNoExiste_DeberiaTirarExcepcion()
    {
        _service.EliminarAtraccionDeEvento(Guid.NewGuid(), Guid.NewGuid());
    }
}
