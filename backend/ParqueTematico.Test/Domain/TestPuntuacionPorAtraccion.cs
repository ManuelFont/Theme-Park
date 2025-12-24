using Dominio.Entities;
using Dominio.Entities.Puntuacion;
using Dominio.Entities.Usuarios;

namespace Test.Domain;

[TestClass]
public class TestPuntuacionPorAtraccion
{
    private PuntuacionPorAtraccion _estrategia = null!;

    [TestInitialize]
    public void SetUp()
    {
        _estrategia = new PuntuacionPorAtraccion();
    }

    [TestMethod]
    public void CalcularPuntos_MontañaRusa_Retorna100()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 0, 50, "Desc", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);
        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now);

        var puntos = _estrategia.CalcularPuntos(acceso, []);

        Assert.AreEqual(100, puntos);
    }

    [TestMethod]
    public void CalcularPuntos_Simulador_Retorna75()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion = new Atraccion("Simulador", TipoAtraccion.Simulador, 0, 50, "Desc", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);
        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now);

        var puntos = _estrategia.CalcularPuntos(acceso, []);

        Assert.AreEqual(75, puntos);
    }

    [TestMethod]
    public void CalcularPuntos_Espectaculo_Retorna50()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion = new Atraccion("Show", TipoAtraccion.Espectáculo, 0, 50, "Desc", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);
        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now);

        var puntos = _estrategia.CalcularPuntos(acceso, []);

        Assert.AreEqual(50, puntos);
    }

    [TestMethod]
    public void CalcularPuntos_ZonaInteractiva_Retorna25()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion = new Atraccion("Zona", TipoAtraccion.ZonaInteractiva, 0, 50, "Desc", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);
        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now);

        var puntos = _estrategia.CalcularPuntos(acceso, []);

        Assert.AreEqual(25, puntos);
    }
}
