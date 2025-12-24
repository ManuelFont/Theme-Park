using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;

namespace Test.Repositories;

[TestClass]
public class HistorialPuntuacionDbRepositoryTest
{
    private ParqueDbContext _context = null!;
    private HistorialPuntuacionDbRepository _repo = null!;
    private UsuarioDbRepository _usuarioRepo = null!;
    private Visitante _visitante = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _repo = new HistorialPuntuacionDbRepository(_context);
        _usuarioRepo = new UsuarioDbRepository(_context);

        _visitante = new Visitante("Juan", "Pérez", "juan@test.com", "Pass123!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(_visitante);
    }

    [TestMethod]
    public void Agregar_HistorialValido_DebeAgregar()
    {
        var historial = new HistorialPuntuacion(_visitante, 100, "Acceso a atracción", "PuntuacionPorAtraccion",
            DateTime.Now);

        _repo.Agregar(historial);

        var resultado = _repo.ObtenerPorVisitante(_visitante.Id);
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(100, resultado[0].Puntos);
        Assert.AreEqual("Acceso a atracción", resultado[0].Origen);
    }

    [TestMethod]
    public void ObtenerPorVisitante_SinHistorial_DebeRetornarListaVacia()
    {
        var resultado = _repo.ObtenerPorVisitante(_visitante.Id);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void ObtenerPorVisitante_ConHistorial_DebeRetornarOrdenadoPorFechaDescendente()
    {
        var fecha1 = new DateTime(2025, 1, 1, 10, 0, 0);
        var fecha2 = new DateTime(2025, 1, 1, 14, 0, 0);
        var fecha3 = new DateTime(2025, 1, 1, 12, 0, 0);

        var historial1 = new HistorialPuntuacion(_visitante, 50, "Acceso 1", "PuntuacionPorAtraccion", fecha1);
        var historial2 = new HistorialPuntuacion(_visitante, 100, "Acceso 2", "PuntuacionPorAtraccion", fecha2);
        var historial3 = new HistorialPuntuacion(_visitante, 75, "Acceso 3", "PuntuacionPorAtraccion", fecha3);

        _repo.Agregar(historial1);
        _repo.Agregar(historial2);
        _repo.Agregar(historial3);

        var resultado = _repo.ObtenerPorVisitante(_visitante.Id);

        Assert.AreEqual(3, resultado.Count);
        Assert.AreEqual(fecha2, resultado[0].FechaHora);
        Assert.AreEqual(100, resultado[0].Puntos);
        Assert.AreEqual(fecha3, resultado[1].FechaHora);
        Assert.AreEqual(75, resultado[1].Puntos);
        Assert.AreEqual(fecha1, resultado[2].FechaHora);
        Assert.AreEqual(50, resultado[2].Puntos);
    }

    [TestMethod]
    public void ObtenerPorVisitante_OtroVisitante_NoDebeRetornarHistorial()
    {
        var otroVisitante = new Visitante("Pedro", "López", "pedro@test.com", "Pass123!", new DateTime(2000, 1, 1),
            NivelMembresia.Premium);
        _usuarioRepo.Agregar(otroVisitante);

        var historial = new HistorialPuntuacion(_visitante, 100, "Acceso", "PuntuacionPorAtraccion", DateTime.Now);
        _repo.Agregar(historial);

        var resultado = _repo.ObtenerPorVisitante(otroVisitante.Id);

        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void Agregar_PuntosNegativos_DebeAgregar()
    {
        var historial = new HistorialPuntuacion(_visitante, -50, "Canje de recompensa",
            "PuntuacionPorAtraccion", DateTime.Now);

        _repo.Agregar(historial);

        var resultado = _repo.ObtenerPorVisitante(_visitante.Id);
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(-50, resultado[0].Puntos);
    }

    [TestMethod]
    public void ObtenerPorVisitante_VisitanteNoExiste_DebeRetornarListaVacia()
    {
        var resultado = _repo.ObtenerPorVisitante(Guid.NewGuid());

        Assert.IsNotNull(resultado);
        Assert.AreEqual(0, resultado.Count);
    }
}
