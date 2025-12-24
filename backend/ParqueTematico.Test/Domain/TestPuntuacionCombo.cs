using Dominio.Entities;
using Dominio.Entities.Puntuacion;
using Dominio.Entities.Usuarios;

namespace Test.Domain;

[TestClass]
public class PuntuacionComboTests
{
    [TestMethod]
    public void CalcularPuntos_SinAccesosPrevios_DeberiaRetornarPuntosBase()
    {
        var strategy = new PuntuacionCombo();
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripción", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);
        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now);

        var puntos = strategy.CalcularPuntos(acceso, []);

        Assert.AreEqual(100, puntos);
    }

    [TestMethod]
    public void CalcularPuntos_ConComboEnTiempo_DeberiaAplicarBonificacion()
    {
        var strategy = new PuntuacionCombo(30, 50);
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion1 = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripción", true);
        var atraccion2 = new Atraccion("Simulador", TipoAtraccion.Simulador, 8, 30, "Descripción", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);

        var fechaBase = new DateTime(2025, 10, 8, 10, 0, 0);
        var acceso1 = new AccesoAtraccion(visitante, atraccion1, ticket, fechaBase);
        acceso1.RegistrarEgreso(fechaBase.AddMinutes(10));

        var acceso2 = new AccesoAtraccion(visitante, atraccion2, ticket, fechaBase.AddMinutes(20));

        var accesos = new List<AccesoAtraccion> { acceso1 };
        var puntos = strategy.CalcularPuntos(acceso2, accesos);

        Assert.AreEqual(125, puntos);
    }

    [TestMethod]
    public void CalcularPuntos_ComboFueraDeTiempo_NoDebeAplicarBonificacion()
    {
        var strategy = new PuntuacionCombo(30, 50);
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion1 = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripción", true);
        var atraccion2 = new Atraccion("Simulador", TipoAtraccion.Simulador, 8, 30, "Descripción", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);

        var fechaBase = new DateTime(2025, 10, 8, 10, 0, 0);
        var acceso1 = new AccesoAtraccion(visitante, atraccion1, ticket, fechaBase);
        acceso1.RegistrarEgreso(fechaBase.AddMinutes(10));

        var acceso2 = new AccesoAtraccion(visitante, atraccion2, ticket, fechaBase.AddMinutes(60));

        var accesos = new List<AccesoAtraccion> { acceso1 };
        var puntos = strategy.CalcularPuntos(acceso2, accesos);

        Assert.AreEqual(75, puntos);
    }

    [TestMethod]
    public void CalcularPuntos_AccesoPrevioMismoTipo_NoDeberiaFormarCombo()
    {
        var strategy = new PuntuacionCombo(30, 50);
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion1 = new Atraccion("Montaña Rusa 1", TipoAtraccion.MontañaRusa, 12, 50, "Descripción", true);
        var atraccion2 = new Atraccion("Montaña Rusa 2", TipoAtraccion.MontañaRusa, 12, 50, "Descripción", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);

        var fechaBase = new DateTime(2025, 10, 8, 10, 0, 0);
        var acceso1 = new AccesoAtraccion(visitante, atraccion1, ticket, fechaBase);
        acceso1.RegistrarEgreso(fechaBase.AddMinutes(10));

        var acceso2 = new AccesoAtraccion(visitante, atraccion2, ticket, fechaBase.AddMinutes(20));

        var accesos = new List<AccesoAtraccion> { acceso1 };
        var puntos = strategy.CalcularPuntos(acceso2, accesos);

        Assert.AreEqual(100, puntos);
    }

    [TestMethod]
    public void CalcularPuntos_AccesoPrevioSinEgreso_NoDeberiaFormarCombo()
    {
        var strategy = new PuntuacionCombo(30, 50);
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion1 = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripción", true);
        var atraccion2 = new Atraccion("Simulador", TipoAtraccion.Simulador, 8, 30, "Descripción", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);

        var fechaBase = new DateTime(2025, 10, 8, 10, 0, 0);
        var acceso1 = new AccesoAtraccion(visitante, atraccion1, ticket, fechaBase);

        var acceso2 = new AccesoAtraccion(visitante, atraccion2, ticket, fechaBase.AddMinutes(20));

        var accesos = new List<AccesoAtraccion> { acceso1 };
        var puntos = strategy.CalcularPuntos(acceso2, accesos);

        Assert.AreEqual(75, puntos);
    }

    [TestMethod]
    public void CalcularPuntos_TipoEspectaculo_DeberiaRetornar50Puntos()
    {
        var strategy = new PuntuacionCombo();
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
        var strategy = new PuntuacionCombo();
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion = new Atraccion("Zona Juegos", TipoAtraccion.ZonaInteractiva, 3, 50, "Descripción", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);
        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now);

        var puntos = strategy.CalcularPuntos(acceso, []);

        Assert.AreEqual(25, puntos);
    }

    [TestMethod]
    public void Nombre_DeberiaRetornarNombreCorrecto()
    {
        var strategy = new PuntuacionCombo();

        Assert.AreEqual("Puntuación Combo", strategy.Nombre);
    }
}
