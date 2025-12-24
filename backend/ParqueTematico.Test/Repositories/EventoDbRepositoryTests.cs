using Dominio.Entities;
using Dominio.Exceptions;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;

namespace Test.Repositories;

[TestClass]
public class EventoDbRepositoryTests
{
    private ParqueDbContext _ctx = null!;
    private EventoDbRepository _repo = null!;

    [TestInitialize]
    public void Setup()
    {
        _ctx = SqlContextFactory.CreateMemoryContext();
        _repo = new EventoDbRepository(_ctx);
    }

    [TestMethod]
    public void Agregar_DeberiaAñadirEvento()
    {
        var evento = new Evento("Evento", DateTime.Today.AddDays(1), new TimeSpan(1, 0, 0), 50, 100);
        _repo.Agregar(evento);

        IEnumerable<Evento> eventos = _repo.ObtenerTodos();
        Assert.AreEqual(evento.Id, eventos.First().Id);
    }

    [TestMethod]
    public void Eliminar_DeberiaBorrarEvento()
    {
        var evento = new Evento("Evento", DateTime.Today.AddDays(1), new TimeSpan(1, 0, 0), 50, 100);
        _repo.Agregar(evento);
        _repo.Eliminar(evento.Id);
        IEnumerable<Evento> eventos = _repo.ObtenerTodos();
        Assert.AreEqual(0, eventos.Count());
    }

    [TestMethod]
    public void Obtener_DeberiaDevolverLosEventos()
    {
        var evento = new Evento("Evento", DateTime.Today.AddDays(1), new TimeSpan(1, 0, 0), 50, 100);
        _repo.Agregar(evento);
        IEnumerable<Evento> eventos = _repo.ObtenerTodos();
        Assert.AreEqual(evento.Id, eventos.First().Id);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void AgregarRepetido_DeberiaTirarExepcion()
    {
        var evento = new Evento("Evento", DateTime.Today.AddDays(1), new TimeSpan(1, 0, 0), 50, 100);
        _repo.Agregar(evento);
        _repo.Agregar(evento);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void EliminarRepetido_DeberiaTirarExepcion()
    {
        _repo.Eliminar(Guid.Empty);
    }

    [TestMethod]
    public void ObtenerPorId_CuandoExiste_DeberiaRetornarEvento()
    {
        var e = new Evento("Evento", DateTime.Today.AddDays(2), new TimeSpan(18, 0, 0), 120, 0m);
        _repo.Agregar(e);

        Evento? obtenido = _repo.ObtenerPorId(e.Id);

        Assert.IsNotNull(obtenido);
        Assert.AreEqual(e.Id, obtenido!.Id);
    }

    [TestMethod]
    public void Actualizar_ModificaEvento()
    {
        var e = new Evento("Evento", DateTime.Today.AddDays(3), new TimeSpan(20, 0, 0), 80, 10m);
        _repo.Agregar(e);

        var eventoActualizado = _repo.ObtenerPorId(e.Id)!;
        eventoActualizado.Nombre = "Evento Actualizado";
        eventoActualizado.Hora = new TimeSpan(21, 0, 0);
        _repo.Actualizar(eventoActualizado);

        var desdeDb = _ctx.Eventos.First(ev => ev.Id == e.Id);
        Assert.AreEqual("Evento Actualizado", desdeDb.Nombre);
        Assert.AreEqual(new TimeSpan(21, 0, 0), desdeDb.Hora);
    }

    [TestMethod]
    public void Agregar_ConAtraccionesExistentes_NoDuplicaAtracciones()
    {
        var a1 = new Atraccion("Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Divertida", true);
        var a2 = new Atraccion("Sim", TipoAtraccion.Simulador, 8, 15, "Emocionante", true);
        _ctx.Atracciones.AddRange(a1, a2);
        _ctx.SaveChanges();
        var e = new Evento("Evento", DateTime.Today.AddDays(3), new TimeSpan(20, 0, 0), 100, 0m);
        e.Atracciones.Add(a1);
        e.Atracciones.Add(a2);

        _repo.Agregar(e);

        Assert.AreEqual(1, _ctx.Eventos.Count());
        Assert.AreEqual(2, _ctx.Atracciones.Count());
        Evento obtenido = _repo.ObtenerPorId(e.Id)!;
        CollectionAssert.AreEquivalent(new[] { a1.Id, a2.Id }, obtenido.Atracciones.Select(x => x.Id).ToArray());
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void Agregar_RepetidoMismoId_DeberiaLanzarEventoException()
    {
        var e = new Evento("Evento", DateTime.Today.AddDays(4), new TimeSpan(19, 0, 0), 80, 10m);
        _repo.Agregar(e);
        _repo.Agregar(e);
    }

    [TestMethod]
    public void ObtenerPorId_CuandoNoExiste_DeberiaRetornarNull()
    {
        Evento? obtenido = _repo.ObtenerPorId(Guid.NewGuid());
        Assert.IsNull(obtenido);
    }

    [TestMethod]
    public void Actualizar_ConAtracciones_SincronizaCorrectamente()
    {
        var a1 = new Atraccion("Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Divertida", true);
        var a2 = new Atraccion("Sim", TipoAtraccion.Simulador, 8, 15, "Emocionante", true);
        _ctx.Atracciones.AddRange(a1, a2);
        _ctx.SaveChanges();

        var e = new Evento("Evento", DateTime.Today.AddDays(8), new TimeSpan(16, 0, 0), 60, 0m);
        _repo.Agregar(e);

        var eventoActualizado = _repo.ObtenerPorId(e.Id)!;
        eventoActualizado.AgregarAtraccion(a1);
        _repo.Actualizar(eventoActualizado);

        var desdeDb = _repo.ObtenerPorId(e.Id)!;
        Assert.AreEqual(1, desdeDb.Atracciones.Count);
        Assert.AreEqual(a1.Id, desdeDb.Atracciones.First().Id);
    }
}
