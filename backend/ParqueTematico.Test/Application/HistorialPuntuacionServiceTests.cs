using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using ParqueTematico.Application.Services;

namespace Test.Application;

[TestClass]
public class HistorialPuntuacionServiceTests
{
    private ParqueDbContext _context = null!;
    private HistorialPuntuacionDbRepository _repository = null!;
    private HistorialPuntuacionService _serviceService = null!;
    private Visitante _visitante = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _repository = new HistorialPuntuacionDbRepository(_context);
        _serviceService = new HistorialPuntuacionService(_repository);

        _visitante = new Visitante("Juan", "Pérez", "juan@test.com", "Pass123!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);

        var usuarioRepo = new UsuarioDbRepository(_context);
        usuarioRepo.Agregar(_visitante);
    }

    [TestMethod]
    public void ObtenerHistorialPorVisitante_SinHistorial_DeberiaRetornarListaVacia()
    {
        IList<HistorialPuntuacion> historial = _serviceService.ObtenerHistorialPorVisitante(_visitante.Id);

        Assert.IsNotNull(historial);
        Assert.AreEqual(0, historial.Count);
    }

    [TestMethod]
    public void RegistrarHistorial_DatosCorrectos_DeberiaGuardar()
    {
        var fechaHora = new DateTime(2025, 10, 24, 15, 30, 0);
        var historial =
            new HistorialPuntuacion(_visitante, 100, "Acceso a atracción", "PuntuacionPorAtraccion", fechaHora);

        _serviceService.RegistrarHistorial(historial);

        IList<HistorialPuntuacion> historiales = _serviceService.ObtenerHistorialPorVisitante(_visitante.Id);
        Assert.AreEqual(1, historiales.Count);
        Assert.AreEqual(100, historiales[0].Puntos);
        Assert.AreEqual("Acceso a atracción", historiales[0].Origen);
    }

    [TestMethod]
    public void ObtenerHistorialPorVisitante_ConVariosRegistros_DeberiaRetornarOrdenadoPorFecha()
    {
        var fecha1 = new DateTime(2025, 10, 24, 10, 0, 0);
        var fecha2 = new DateTime(2025, 10, 24, 14, 0, 0);
        var fecha3 = new DateTime(2025, 10, 24, 12, 0, 0);

        var historial1 = new HistorialPuntuacion(_visitante, 50, "Acceso 1", "PuntuacionPorAtraccion", fecha1);
        var historial2 = new HistorialPuntuacion(_visitante, 100, "Acceso 2", "PuntuacionPorAtraccion", fecha2);
        var historial3 = new HistorialPuntuacion(_visitante, 75, "Acceso 3", "PuntuacionPorAtraccion", fecha3);

        _serviceService.RegistrarHistorial(historial1);
        _serviceService.RegistrarHistorial(historial2);
        _serviceService.RegistrarHistorial(historial3);

        IList<HistorialPuntuacion> historiales = _serviceService.ObtenerHistorialPorVisitante(_visitante.Id);

        Assert.AreEqual(3, historiales.Count);
        Assert.AreEqual(fecha2, historiales[0].FechaHora);
        Assert.AreEqual(fecha3, historiales[1].FechaHora);
        Assert.AreEqual(fecha1, historiales[2].FechaHora);
    }
}
