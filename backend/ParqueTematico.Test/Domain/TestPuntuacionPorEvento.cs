using Dominio.Entities;
using Dominio.Entities.Puntuacion;
using Dominio.Entities.Usuarios;

namespace Test.Domain;

[TestClass]
public class PuntuacionPorEventoTests
{
    [TestMethod]
    public void CalcularPuntos_TicketGeneral_DeberiaRetornarPuntosBase()
    {
        var strategy = new PuntuacionPorEvento(1.5m);
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripción", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);
        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now);

        var puntos = strategy.CalcularPuntos(acceso, []);

        Assert.AreEqual(100, puntos);
    }

    [TestMethod]
    public void CalcularPuntos_TicketEventoEspecial_DeberiaAplicarMultiplicador()
    {
        var strategy = new PuntuacionPorEvento(1.5m);
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var evento = new Evento("Festival", DateTime.Now.AddDays(1), new TimeSpan(18, 0, 0), 100, 500);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripción", true);
        evento.AgregarAtraccion(atraccion);
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.EventoEspecial, evento);
        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now.AddDays(1));

        var puntos = strategy.CalcularPuntos(acceso, []);

        Assert.AreEqual(150, puntos);
    }

    [TestMethod]
    public void CalcularPuntos_TipoSimulador_DeberiaRetornar75Puntos()
    {
        var strategy = new PuntuacionPorEvento();
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion = new Atraccion("Simulador VR", TipoAtraccion.Simulador, 8, 30, "Descripción", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);
        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now);

        var puntos = strategy.CalcularPuntos(acceso, []);

        Assert.AreEqual(75, puntos);
    }

    [TestMethod]
    public void CalcularPuntos_TipoEspectaculo_DeberiaRetornar50Puntos()
    {
        var strategy = new PuntuacionPorEvento();
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion = new Atraccion("Show Musical", TipoAtraccion.Espectáculo, 5, 100, "Descripción", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);
        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now);

        var puntos = strategy.CalcularPuntos(acceso, []);

        Assert.AreEqual(50, puntos);
    }

    [TestMethod]
    public void CalcularPuntos_TipoZonaInteractiva_DeberiaRetornar25Puntos()
    {
        var strategy = new PuntuacionPorEvento();
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion = new Atraccion("Zona Juegos", TipoAtraccion.ZonaInteractiva, 3, 50, "Descripción", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);
        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now);

        var puntos = strategy.CalcularPuntos(acceso, []);

        Assert.AreEqual(25, puntos);
    }

    [TestMethod]
    public void CalcularPuntos_EventoEspecialConMultiplicadorPersonalizado_DeberiaAplicarlo()
    {
        var strategy = new PuntuacionPorEvento(2.0m);
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var evento = new Evento("Festival", DateTime.Now.AddDays(1), new TimeSpan(18, 0, 0), 100, 500);
        var atraccion = new Atraccion("Simulador VR", TipoAtraccion.Simulador, 8, 30, "Descripción", true);
        evento.AgregarAtraccion(atraccion);
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.EventoEspecial, evento);
        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now.AddDays(1));

        var puntos = strategy.CalcularPuntos(acceso, []);

        Assert.AreEqual(150, puntos);
    }

    [TestMethod]
    public void Nombre_DeberiaRetornarNombreCorrecto()
    {
        var strategy = new PuntuacionPorEvento();

        Assert.AreEqual("Puntuación Por Evento", strategy.Nombre);
    }
}
