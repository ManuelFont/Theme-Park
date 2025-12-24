using Dominio.Entities;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using ParqueTematico.Application.Services;

namespace Test.Application;

[TestClass]
public class MantenimientoPreventivoServiceTests
{
    private Guid _atraccionId = Guid.Empty;
    private ParqueDbContext _context = null!;
    private RelojService _reloj = null!;
    private RelojDbRepository _relojDbRepository = null!;
    private BaseRepository<Atraccion> _repoAtracciones = null!;
    private IncidenciaDbRepository _repository = null!;
    private MantenimientoPreventivoService _service = null!;
    private MantenimientoPreventivoDbRepository _mantenimientoRepository = null!;
    private AtraccionService _atraccionService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _repository = new IncidenciaDbRepository(_context);
        _repoAtracciones = new BaseRepository<Atraccion>(_context);
        _atraccionService = new AtraccionService(_repoAtracciones);
        _relojDbRepository = new RelojDbRepository(_context);
        _reloj = new RelojService(_relojDbRepository);
        _mantenimientoRepository = new MantenimientoPreventivoDbRepository(_context);
        _service = new MantenimientoPreventivoService(_mantenimientoRepository, _reloj, _atraccionService);
        _atraccionId = RegistrarAtraccion("montaniaLoca", TipoAtraccion.MontañaRusa, 12, 10, "des");
    }

    private Guid RegistrarAtraccion(string nombre, TipoAtraccion tipo, int edadMin, int capacidad, string des)
    {
        var atraccion = new Atraccion(nombre, tipo, edadMin, capacidad, des, true);
        _repoAtracciones.Agregar(atraccion);
        return atraccion.Id;
    }

    [TestMethod]
    public void Constructor_ValoresCorrectos_CrearService()
    {
        var service = new MantenimientoPreventivoService(_mantenimientoRepository, _reloj, _atraccionService);
        Assert.IsNotNull(service);
    }

    [TestMethod]
    public void Crear_FechaActual_MantenimientoActivoRegistradaEnRepo()
    {
        var fechaInicio = new DateTime(2026, 1, 1);
        var fechaReloj = new DateTime(2026, 1, 2);
        var fechaFin = new DateTime(2026, 1, 3);
        _reloj.ModificarFechaHora(fechaReloj);
        MantenimientoPreventivo mantenimiento = _service.Crear(_atraccionId, "des", fechaInicio, fechaFin);
        Assert.IsTrue(mantenimiento.EstaActiva);
        Assert.IsNotNull(_repository.ObtenerPorId(mantenimiento.Id));
    }

    [TestMethod]
    public void Crear_FechaNoActual_MantenimientoNoActivoRegistradoEnRepo()
    {
        var fechaInicio = new DateTime(2026, 1, 1);
        var fechaReloj = new DateTime(2026, 1, 5);
        var fechaFin = new DateTime(2026, 1, 3);
        _reloj.ModificarFechaHora(fechaReloj);
        MantenimientoPreventivo mantenimiento = _service.Crear(_atraccionId, "des", fechaInicio, fechaFin);
        Assert.IsFalse(mantenimiento.EstaActiva);
        Assert.IsNotNull(_repository.ObtenerPorId(mantenimiento.Id));
    }

    [TestMethod]
    public void Crear_DatosValidos_MantenimientoRegistradoConTodosLosDatos()
    {
        var fechaInicio = new DateTime(2026, 1, 1);
        var fechaFin = new DateTime(2026, 1, 3);
        MantenimientoPreventivo mantenimiento = _service.Crear(_atraccionId, "des", fechaInicio, fechaFin);
        var mantenimientoDb = (MantenimientoPreventivo)_repository.ObtenerPorId(mantenimiento.Id)!;
        Assert.AreEqual(fechaInicio, mantenimientoDb.FechaInicio);
        Assert.AreEqual(fechaFin, mantenimientoDb.FechaFin);
    }

    [TestMethod]
    public void ModificacionHora_ActivarODesactivarSegunCorresponda()
    {
        var fechaInicio = new DateTime(2026, 1, 1);
        _reloj.ModificarFechaHora(new DateTime(2026, 1, 2));
        var fechaFin = new DateTime(2026, 1, 3);
        MantenimientoPreventivo mantenimiento = _service.Crear(_atraccionId, "des", fechaInicio, fechaFin);
        Assert.IsTrue(_repository.ObtenerPorId(mantenimiento.Id)!.EstaActiva);
        _reloj.ModificarFechaHora(new DateTime(2030, 1, 1));
        Assert.IsFalse(_repository.ObtenerPorId(mantenimiento.Id)!.EstaActiva);
        _reloj.ModificarFechaHora(new DateTime(2026, 1, 2));
        Assert.IsTrue(_repository.ObtenerPorId(mantenimiento.Id)!.EstaActiva);
        _reloj.ModificarFechaHora(new DateTime(2000, 1, 1));
        Assert.IsFalse(_repository.ObtenerPorId(mantenimiento.Id)!.EstaActiva);
    }
}
