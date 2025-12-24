using Dominio.Entities;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;

namespace Test.Repositories;

[TestClass]
public class IncidenciaDbRepositoryTest
{
    private ParqueDbContext _context = null!;
    private IncidenciaDbRepository _repo = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _repo = new IncidenciaDbRepository(_context);
    }

    [TestMethod]
    public void Agregar_GuardaIncidenciaConAtraccion()
    {
        Atraccion atr = CrearAtraccionValida();
        _context.Atracciones.Add(atr);
        _context.SaveChanges();

        var incidencia = new Incidencia(atr, TipoIncidencia.FueraDeServicio, "Motor detenido", true);
        Incidencia resultado = _repo.Agregar(incidencia);

        Assert.IsNotNull(resultado);
        Assert.AreNotEqual(Guid.Empty, resultado.Id);
        Assert.IsTrue(resultado.EstaActiva);
        Assert.AreEqual(atr.Id, resultado.Atraccion.Id);

        Incidencia? desdeDb = _context.Incidencias.FirstOrDefault(i => i.Id == resultado.Id);
        Assert.IsNotNull(desdeDb);
    }

    [TestMethod]
    public void ObtenerPorId_CuandoExiste_DevuelveLaIncidencia()
    {
        Atraccion atr = CrearAtraccionValida();
        _context.Atracciones.Add(atr);
        _context.SaveChanges();

        var incidencia = new Incidencia(atr, TipoIncidencia.Mantenimiento, "Correas", true);
        _context.Incidencias.Add(incidencia);
        _context.SaveChanges();

        Incidencia? resultado = _repo.ObtenerPorId(incidencia.Id);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(incidencia.Id, resultado!.Id);
    }

    [TestMethod]
    public void ObtenerPorId_CuandoNoExiste_DevuelveNull()
    {
        Incidencia? resultado = _repo.ObtenerPorId(Guid.NewGuid());
        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void Actualizar_ModificaIncidencia()
    {
        Atraccion atr = CrearAtraccionValida();
        _context.Atracciones.Add(atr);
        _context.SaveChanges();

        var incidencia = new Incidencia(atr, TipoIncidencia.Mantenimiento, "Correas", true);
        _context.Incidencias.Add(incidencia);
        _context.SaveChanges();

        incidencia.Desactivar();
        _repo.Actualizar(incidencia);

        var desdeDb = _context.Incidencias.First(i => i.Id == incidencia.Id);
        Assert.IsFalse(desdeDb.EstaActiva);
    }

    [TestMethod]
    public void ExisteActiva_ConIncidenciaActiva_RetornaTrue()
    {
        Atraccion atr = CrearAtraccionValida();
        _context.Atracciones.Add(atr);
        _context.SaveChanges();

        var incidencia = new Incidencia(atr, TipoIncidencia.Mantenimiento, "Correas", true);
        _context.Incidencias.Add(incidencia);
        _context.SaveChanges();

        var hayActiva = _repo.ExisteActiva(atr.Id);
        Assert.IsTrue(hayActiva);
    }

    [TestMethod]
    public void ExisteActiva_SinActivas_RetornaFalse()
    {
        Atraccion atr = CrearAtraccionValida();
        _context.Atracciones.Add(atr);
        _context.SaveChanges();

        var incidencia = new Incidencia(atr, TipoIncidencia.Mantenimiento, "Correas", false);
        _context.Incidencias.Add(incidencia);
        _context.SaveChanges();

        var hayActiva = _repo.ExisteActiva(atr.Id);
        Assert.IsFalse(hayActiva);
    }

    [TestMethod]
    public void ObtenerActivasPorAtraccion_SoloDevuelveActivasDeEsaAtraccion()
    {
        Atraccion a1 = CrearAtraccionValida();
        var a2 = new Atraccion("Simulador X", TipoAtraccion.Simulador, 10, 12, "VR", true);
        _context.Atracciones.AddRange(a1, a2);
        _context.SaveChanges();

        var inc1 = new Incidencia(a1, TipoIncidencia.Mantenimiento, "Correas", true);
        var inc2 = new Incidencia(a1, TipoIncidencia.Rota, "Eje", false);
        var inc3 = new Incidencia(a2, TipoIncidencia.FueraDeServicio, "Software", true);
        _context.Incidencias.AddRange(inc1, inc2, inc3);
        _context.SaveChanges();

        IList<Incidencia> activas = _repo.ObtenerActivasPorAtraccion(a1.Id);

        Assert.AreEqual(1, activas.Count);
        Assert.AreEqual(inc1.Id, activas[0].Id);
    }

    private Atraccion CrearAtraccionValida()
    {
        return new Atraccion(
            "Montaña Rusa",
            TipoAtraccion.MontañaRusa,
            12,
            24,
            "Principal del parque", true);
    }
}
