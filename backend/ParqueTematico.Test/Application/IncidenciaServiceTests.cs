using Dominio.Entities;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using ParqueTematico.Application.Services;
using RepositoryInterfaces;

namespace Test.Application;

[TestClass]
public class IncidenciaServiceTests
{
    private IAtraccionRepository _atraccionRepo = null!;
    private AtraccionService _atraccionService = null!;
    private ParqueDbContext _ctx = null!;
    private IncidenciaDbRepository _incidenciaRepo = null!;
    private IncidenciaService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _ctx = SqlContextFactory.CreateMemoryContext();
        _incidenciaRepo = new IncidenciaDbRepository(_ctx);
        _atraccionRepo = new AtraccionRepository(_ctx);
        _atraccionService = new AtraccionService(_atraccionRepo);
        _service = new IncidenciaService(_incidenciaRepo, _atraccionService);
    }

    [TestMethod]
    public void Crear_ConDatosValidos_DeberiaCrearIncidenciaActiva()
    {
        _atraccionService.Crear("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        Atraccion atraccion = _atraccionRepo.ObtenerTodos().First();

        Incidencia incidencia = _service.Crear(atraccion.Id, TipoIncidencia.Mantenimiento, "Cambio de correas");

        Assert.IsNotNull(incidencia);
        Assert.AreEqual(atraccion.Id, incidencia.Atraccion.Id);
        Assert.AreEqual(TipoIncidencia.Mantenimiento, incidencia.TipoIncidencia);
        Assert.AreEqual("Cambio de correas", incidencia.Descripcion);
        Assert.IsTrue(incidencia.EstaActiva);
    }

    [TestMethod]
    public void Cerrar_IncidenciaExistente_DeberiaDesactivarla()
    {
        _atraccionService.Crear("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        Atraccion atraccion = _atraccionRepo.ObtenerTodos().First();
        Incidencia incidencia = _service.Crear(atraccion.Id, TipoIncidencia.Mantenimiento, "Cambio de correas");

        _service.Cerrar(incidencia.Id);

        Incidencia? incidenciaCerrada = _incidenciaRepo.ObtenerPorId(incidencia.Id);
        Assert.IsNotNull(incidenciaCerrada);
        Assert.IsFalse(incidenciaCerrada.EstaActiva);
    }

    [TestMethod]
    public void ExisteActiva_ConIncidenciaActiva_DeberiaRetornarTrue()
    {
        _atraccionService.Crear("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        Atraccion atraccion = _atraccionRepo.ObtenerTodos().First();
        _service.Crear(atraccion.Id, TipoIncidencia.Mantenimiento, "Cambio de correas");

        var existe = _service.ExisteActiva(atraccion.Id);

        Assert.IsTrue(existe);
    }

    [TestMethod]
    public void ExisteActiva_SinIncidencias_DeberiaRetornarFalse()
    {
        _atraccionService.Crear("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        Atraccion atraccion = _atraccionRepo.ObtenerTodos().First();

        var existe = _service.ExisteActiva(atraccion.Id);

        Assert.IsFalse(existe);
    }

    [TestMethod]
    public void ExisteActiva_ConIncidenciaCerrada_DeberiaRetornarFalse()
    {
        _atraccionService.Crear("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        Atraccion atraccion = _atraccionRepo.ObtenerTodos().First();
        Incidencia incidencia = _service.Crear(atraccion.Id, TipoIncidencia.Mantenimiento, "Cambio de correas");
        _service.Cerrar(incidencia.Id);

        var existe = _service.ExisteActiva(atraccion.Id);

        Assert.IsFalse(existe);
    }

    [TestMethod]
    public void ObtenerActivasPorAtraccion_DeberiaRetornarSoloActivas()
    {
        _atraccionService.Crear("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        Atraccion atraccion = _atraccionRepo.ObtenerTodos().First();

        Incidencia inc1 = _service.Crear(atraccion.Id, TipoIncidencia.Mantenimiento, "Correas");
        Incidencia inc2 = _service.Crear(atraccion.Id, TipoIncidencia.Rota, "Eje");
        _service.Cerrar(inc2.Id);

        IList<Incidencia> activas = _service.ObtenerActivasPorAtraccion(atraccion.Id);

        Assert.AreEqual(1, activas.Count);
        Assert.AreEqual(inc1.Id, activas[0].Id);
    }
}
