using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;

namespace Test.Repositories;

[TestClass]
public class AccesoAtraccionDbRepositoryTest
{
    private ParqueDbContext _context = null!;
    private AccesoAtraccionDbRepository _repo = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _repo = new AccesoAtraccionDbRepository(_context);
    }

    [TestMethod]
    public void Agregar_DebeGuardarAccesoEnBaseDeDatos()
    {
        Visitante visitante = CrearVisitanteValido();
        Atraccion atraccion = CrearAtraccionValida();
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);

        _context.Visitantes.Add(visitante);
        _context.Atracciones.Add(atraccion);
        _context.Tickets.Add(ticket);
        _context.SaveChanges();

        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now);
        _repo.Agregar(acceso);

        AccesoAtraccion? desdeDb = _context.AccesosAtraccion.FirstOrDefault(a => a.Id == acceso.Id);
        Assert.IsNotNull(desdeDb);
        Assert.AreEqual(acceso.Id, desdeDb.Id);
    }

    private Visitante CrearVisitanteValido()
    {
        return new Visitante(
            "Juan",
            "Pérez",
            "juan@test.com",
            "Pass123!",
            new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
    }

    private Atraccion CrearAtraccionValida()
    {
        return new Atraccion(
            "Montaña Rusa",
            TipoAtraccion.MontañaRusa,
            12,
            30,
            "Descripción", true);
    }

    [TestMethod]
    public void ObtenerPorId_CuandoExiste_DevuelveElAcceso()
    {
        Visitante visitante = CrearVisitanteValido();
        Atraccion atraccion = CrearAtraccionValida();
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);

        _context.Visitantes.Add(visitante);
        _context.Atracciones.Add(atraccion);
        _context.Tickets.Add(ticket);
        _context.SaveChanges();

        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now);
        _repo.Agregar(acceso);

        AccesoAtraccion? desdeRepo = _repo.ObtenerPorId(acceso.Id);

        Assert.IsNotNull(desdeRepo);
        Assert.AreEqual(acceso.Id, desdeRepo.Id);
    }

    [TestMethod]
    public void ObtenerPorId_CuandoNoExiste_DevuelveNull()
    {
        AccesoAtraccion? desdeRepo = _repo.ObtenerPorId(Guid.NewGuid());
        Assert.IsNull(desdeRepo);
    }

    [TestMethod]
    public void Actualizar_DebeGuardarCambiosEnBaseDeDatos()
    {
        Visitante visitante = CrearVisitanteValido();
        Atraccion atraccion = CrearAtraccionValida();
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);

        _context.Visitantes.Add(visitante);
        _context.Atracciones.Add(atraccion);
        _context.Tickets.Add(ticket);
        _context.SaveChanges();

        DateTime fechaIngreso = DateTime.Now;
        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, fechaIngreso);
        _repo.Agregar(acceso);

        acceso.RegistrarEgreso(fechaIngreso.AddMinutes(30));
        acceso.AsignarPuntos(100);
        _repo.Actualizar(acceso);

        AccesoAtraccion? desdeDb = _context.AccesosAtraccion.Find(acceso.Id);
        Assert.IsNotNull(desdeDb);
        Assert.IsNotNull(desdeDb.FechaHoraEgreso);
        Assert.AreEqual(100, desdeDb.PuntosObtenidos);
    }

    [TestMethod]
    public void ObtenerAccesosSinEgresoPorAtraccion_SoloDevuelveAccesosSinEgreso()
    {
        Visitante visitante = CrearVisitanteValido();
        Atraccion atraccion = CrearAtraccionValida();
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);

        _context.Visitantes.Add(visitante);
        _context.Atracciones.Add(atraccion);
        _context.Tickets.Add(ticket);
        _context.SaveChanges();

        DateTime fechaIngreso = DateTime.Now;
        var acceso1 = new AccesoAtraccion(visitante, atraccion, ticket, fechaIngreso);
        var acceso2 = new AccesoAtraccion(visitante, atraccion, ticket, fechaIngreso.AddMinutes(10));
        var acceso3 = new AccesoAtraccion(visitante, atraccion, ticket, fechaIngreso.AddMinutes(20));

        _repo.Agregar(acceso1);
        _repo.Agregar(acceso2);
        _repo.Agregar(acceso3);

        acceso2.RegistrarEgreso(fechaIngreso.AddMinutes(40));
        _repo.Actualizar(acceso2);

        IList<AccesoAtraccion> accesosSinEgreso = _repo.ObtenerAccesosSinEgresoPorAtraccion(atraccion.Id);

        Assert.AreEqual(2, accesosSinEgreso.Count);
        Assert.IsTrue(accesosSinEgreso.Any(a => a.Id == acceso1.Id));
        Assert.IsTrue(accesosSinEgreso.Any(a => a.Id == acceso3.Id));
        Assert.IsFalse(accesosSinEgreso.Any(a => a.Id == acceso2.Id));
    }

    [TestMethod]
    public void ObtenerAccesosPorVisitanteYFecha_DevuelveSoloAccesosDelVisitanteEnEsaFecha()
    {
        Visitante visitante1 = CrearVisitanteValido();
        var visitante2 = new Visitante("Maria", "Lopez", "maria@test.com", "Pass123!", new DateTime(1995, 5, 5),
            NivelMembresia.Premium);
        Atraccion atraccion = CrearAtraccionValida();
        var ticket1 = new Ticket(visitante1, DateTime.Now.AddDays(1), TipoEntrada.General, null);
        var ticket2 = new Ticket(visitante2, DateTime.Now.AddDays(1), TipoEntrada.General, null);

        _context.Visitantes.Add(visitante1);
        _context.Visitantes.Add(visitante2);
        _context.Atracciones.Add(atraccion);
        _context.Tickets.Add(ticket1);
        _context.Tickets.Add(ticket2);
        _context.SaveChanges();

        var fecha1 = new DateTime(2025, 9, 30, 14, 0, 0);
        var fecha2 = new DateTime(2025, 10, 1, 14, 0, 0);

        var acceso1 = new AccesoAtraccion(visitante1, atraccion, ticket1, fecha1);
        var acceso2 = new AccesoAtraccion(visitante1, atraccion, ticket1, fecha1.AddHours(1));
        var acceso3 = new AccesoAtraccion(visitante2, atraccion, ticket2, fecha1);
        var acceso4 = new AccesoAtraccion(visitante1, atraccion, ticket1, fecha2);

        _repo.Agregar(acceso1);
        _repo.Agregar(acceso2);
        _repo.Agregar(acceso3);
        _repo.Agregar(acceso4);

        acceso1.AsignarPuntos(50);
        acceso2.AsignarPuntos(75);
        _repo.Actualizar(acceso1);
        _repo.Actualizar(acceso2);

        IList<AccesoAtraccion> accesosVisitante1Fecha1 =
            _repo.ObtenerAccesosPorVisitanteYFecha(visitante1.Id, fecha1.Date);

        Assert.AreEqual(2, accesosVisitante1Fecha1.Count);
        Assert.IsTrue(accesosVisitante1Fecha1.Any(a => a.Id == acceso1.Id));
        Assert.IsTrue(accesosVisitante1Fecha1.Any(a => a.Id == acceso2.Id));
        Assert.IsFalse(accesosVisitante1Fecha1.Any(a => a.Id == acceso3.Id));
        Assert.IsFalse(accesosVisitante1Fecha1.Any(a => a.Id == acceso4.Id));
    }
}
