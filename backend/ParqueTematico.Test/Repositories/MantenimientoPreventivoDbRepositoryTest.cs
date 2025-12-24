using Dominio.Entities;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;

namespace Test.Repositories;

[TestClass]
public class MantenimientoPreventivoDbRepositoryTest
{
    private ParqueDbContext _context = null!;
    private MantenimientoPreventivoDbRepository _repo = null!;
    private BaseRepository<Atraccion> _atraccionRepo = null!;
    private Atraccion _atraccion = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _repo = new MantenimientoPreventivoDbRepository(_context);
        _atraccionRepo = new BaseRepository<Atraccion>(_context);

        _atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripción", true);
        _atraccionRepo.Agregar(_atraccion);
    }

    [TestMethod]
    public void Agregar_GuardaMantenimientoConAtraccion()
    {
        var descripcion = "Cambio de correas";
        var fechaInicio = new DateTime(2026, 1, 1);
        var fechaFin = new DateTime(2026, 1, 2);

        var mantenimiento = new MantenimientoPreventivo(_atraccion, descripcion, false, fechaInicio, fechaFin);
        MantenimientoPreventivo resultado = _repo.Agregar(mantenimiento);

        Assert.IsNotNull(resultado);
        Assert.AreNotEqual(Guid.Empty, resultado.Id);
        Assert.AreEqual(descripcion, resultado.Descripcion);
        Assert.AreEqual(fechaInicio, resultado.FechaInicio);
        Assert.AreEqual(fechaFin, resultado.FechaFin);
        Assert.IsFalse(resultado.EstaActiva);

        MantenimientoPreventivo? desdeDb = _context.Incidencias.OfType<MantenimientoPreventivo>().FirstOrDefault(m => m.Id == resultado.Id);
        Assert.IsNotNull(desdeDb);
    }

    [TestMethod]
    public void ObtenerPorId_CuandoExiste_DevuelveElMantenimiento()
    {
        var mantenimiento = new MantenimientoPreventivo(_atraccion, "Descripción", false, new DateTime(2026, 1, 1), new DateTime(2026, 1, 2));
        _context.Incidencias.Add(mantenimiento);
        _context.SaveChanges();

        MantenimientoPreventivo? resultado = _repo.ObtenerPorId(mantenimiento.Id);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(mantenimiento.Id, resultado!.Id);
        Assert.AreEqual("Descripción", resultado.Descripcion);
        Assert.IsNotNull(resultado.Atraccion);
    }

    [TestMethod]
    public void ObtenerPorId_CuandoNoExiste_DevuelveNull()
    {
        MantenimientoPreventivo? resultado = _repo.ObtenerPorId(Guid.NewGuid());
        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void ObtenerTodos_SinMantenimientos_DebeRetornarListaVacia()
    {
        var resultado = _repo.ObtenerTodos().ToList();

        Assert.IsNotNull(resultado);
        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void ObtenerTodos_ConMantenimientos_DevuelveTodos()
    {
        var fecha1 = new DateTime(2026, 1, 1);
        var fecha2 = new DateTime(2026, 1, 3);
        var fecha3 = new DateTime(2026, 1, 2);

        var m1 = new MantenimientoPreventivo(_atraccion, "Mantenimiento 1", false, fecha1, fecha1.AddDays(1));
        var m2 = new MantenimientoPreventivo(_atraccion, "Mantenimiento 2", false, fecha2, fecha2.AddDays(1));
        var m3 = new MantenimientoPreventivo(_atraccion, "Mantenimiento 3", false, fecha3, fecha3.AddDays(1));
        _context.Incidencias.AddRange(m1, m2, m3);
        _context.SaveChanges();

        var resultado = _repo.ObtenerTodos().ToList();

        Assert.AreEqual(3, resultado.Count);
    }

    [TestMethod]
    public void Actualizar_ModificaMantenimiento()
    {
        var mantenimiento = new MantenimientoPreventivo(_atraccion, "Descripción", false, new DateTime(2026, 1, 1), new DateTime(2026, 1, 2));
        _context.Incidencias.Add(mantenimiento);
        _context.SaveChanges();

        mantenimiento.Activar();
        _repo.Actualizar(mantenimiento);

        var desdeDb = _context.Incidencias.OfType<MantenimientoPreventivo>().First(m => m.Id == mantenimiento.Id);
        Assert.IsTrue(desdeDb.EstaActiva);
    }
}
