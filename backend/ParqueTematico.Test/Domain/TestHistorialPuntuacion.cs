using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Dominio.Exceptions;

namespace Test.Domain;

[TestClass]
public class TestHistorialPuntuacion
{
    private string _estrategia = null!;
    private DateTime _fechaHora;
    private string _origen = null!;
    private int _puntos;
    private Visitante _visitante = null!;

    [TestInitialize]
    public void SetUp()
    {
        _visitante = new Visitante("Juan", "Pérez", "juan@test.com", "Pass123!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _fechaHora = new DateTime(2025, 10, 20, 15, 30, 0);
        _puntos = 100;
        _origen = "Acceso a atracción";
        _estrategia = "PuntuacionPorAtraccion";
    }

    [TestMethod]
    public void CrearHistorialCorrectoDebeAsignarPropiedades()
    {
        var historial = new HistorialPuntuacion(_visitante, _puntos, _origen, _estrategia, _fechaHora);

        Assert.IsNotNull(historial.Id);
        Assert.AreEqual(_visitante, historial.Visitante);
        Assert.AreEqual(_puntos, historial.Puntos);
        Assert.AreEqual(_origen, historial.Origen);
        Assert.AreEqual(_estrategia, historial.EstrategiaActiva);
        Assert.AreEqual(_fechaHora, historial.FechaHora);
    }

    [TestMethod]
    [ExpectedException(typeof(HistorialPuntuacionException))]
    public void CrearHistorialConVisitanteNuloDebeLanzarExcepcion()
    {
        _ = new HistorialPuntuacion(null!, _puntos, _origen, _estrategia, _fechaHora);
    }

    [TestMethod]
    [ExpectedException(typeof(HistorialPuntuacionException))]
    public void CrearHistorialConOrigenNuloDebeLanzarExcepcion()
    {
        _ = new HistorialPuntuacion(_visitante, _puntos, null!, _estrategia, _fechaHora);
    }

    [TestMethod]
    [ExpectedException(typeof(HistorialPuntuacionException))]
    public void CrearHistorialConOrigenVacioDebeLanzarExcepcion()
    {
        _ = new HistorialPuntuacion(_visitante, _puntos, string.Empty, _estrategia, _fechaHora);
    }

    [TestMethod]
    [ExpectedException(typeof(HistorialPuntuacionException))]
    public void CrearHistorialConEstrategiaNulaDebeLanzarExcepcion()
    {
        _ = new HistorialPuntuacion(_visitante, _puntos, _origen, null!, _fechaHora);
    }

    [TestMethod]
    [ExpectedException(typeof(HistorialPuntuacionException))]
    public void CrearHistorialConEstrategiaVaciaDebeLanzarExcepcion()
    {
        _ = new HistorialPuntuacion(_visitante, _puntos, _origen, string.Empty, _fechaHora);
    }
}
