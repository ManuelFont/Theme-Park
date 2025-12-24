using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Dominio.Exceptions;

namespace Test.Domain;

[TestClass]
public class TestAccesoAtraccion
{
    [TestMethod]
    [ExpectedException(typeof(AccesoAtraccionException))]
    public void CrearAccesoConVisitanteNuloDebeLanzarExcepcion()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        var visitante = new Visitante("Juan", "Pérez", "juan@test.com", "Pass123!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);

        new AccesoAtraccion(null!, atraccion, ticket, DateTime.Now);
    }

    [TestMethod]
    [ExpectedException(typeof(AccesoAtraccionException))]
    public void CrearAccesoConAtraccionNulaDebeLanzarExcepcion()
    {
        var visitante = new Visitante("Juan", "Pérez", "juan@test.com", "Pass123!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);

        new AccesoAtraccion(visitante, null!, ticket, DateTime.Now);
    }

    [TestMethod]
    [ExpectedException(typeof(AccesoAtraccionException))]
    public void CrearAccesoConTicketNuloDebeLanzarExcepcion()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        var visitante = new Visitante("Juan", "Pérez", "juan@test.com", "Pass123!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);

        new AccesoAtraccion(visitante, atraccion, null!, DateTime.Now);
    }

    [TestMethod]
    public void CrearAccesoCorrectoDebeAsignarPropiedades()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        var visitante = new Visitante("Juan", "Pérez", "juan@test.com", "Pass123!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);
        DateTime fechaIngreso = DateTime.Now;

        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, fechaIngreso);

        Assert.IsNotNull(acceso.Id);
        Assert.AreEqual(visitante, acceso.Visitante);
        Assert.AreEqual(atraccion, acceso.Atraccion);
        Assert.AreEqual(ticket, acceso.Ticket);
        Assert.AreEqual(fechaIngreso, acceso.FechaHoraIngreso);
        Assert.IsNull(acceso.FechaHoraEgreso);
        Assert.AreEqual(0, acceso.PuntosObtenidos);
    }

    [TestMethod]
    [ExpectedException(typeof(AccesoAtraccionException))]
    public void RegistrarEgresoYaRegistradoDebeLanzarExcepcion()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        var visitante = new Visitante("Juan", "Pérez", "juan@test.com", "Pass123!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);
        DateTime fechaIngreso = DateTime.Now;

        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, fechaIngreso);
        acceso.RegistrarEgreso(fechaIngreso.AddMinutes(30));
        acceso.RegistrarEgreso(fechaIngreso.AddMinutes(40));
    }

    [TestMethod]
    [ExpectedException(typeof(AccesoAtraccionException))]
    public void RegistrarEgresoAnteriorAIngresoDebeLanzarExcepcion()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        var visitante = new Visitante("Juan", "Pérez", "juan@test.com", "Pass123!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);
        DateTime fechaIngreso = DateTime.Now;

        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, fechaIngreso);
        acceso.RegistrarEgreso(fechaIngreso.AddMinutes(-10));
    }

    [TestMethod]
    public void RegistrarEgresoCorrectoDebeAsignarFecha()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        var visitante = new Visitante("Juan", "Pérez", "juan@test.com", "Pass123!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);
        DateTime fechaIngreso = DateTime.Now;
        DateTime fechaEgreso = fechaIngreso.AddMinutes(30);

        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, fechaIngreso);
        acceso.RegistrarEgreso(fechaEgreso);

        Assert.AreEqual(fechaEgreso, acceso.FechaHoraEgreso);
    }

    [TestMethod]
    [ExpectedException(typeof(AccesoAtraccionException))]
    public void AsignarPuntosNegativosDebeLanzarExcepcion()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        var visitante = new Visitante("Juan", "Pérez", "juan@test.com", "Pass123!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);

        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now);
        acceso.AsignarPuntos(-10);
    }

    [TestMethod]
    public void AsignarPuntosCorrectoDebeActualizarValor()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        var visitante = new Visitante("Juan", "Pérez", "juan@test.com", "Pass123!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);

        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now);
        acceso.AsignarPuntos(100);

        Assert.AreEqual(100, acceso.PuntosObtenidos);
    }

    [TestMethod]
    public void EstaActivoDebeDevolverTrueCuandoNoHayEgreso()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        var visitante = new Visitante("Juan", "Pérez", "juan@test.com", "Pass123!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);

        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now);

        Assert.IsTrue(acceso.EstaActivo);
    }

    [TestMethod]
    public void EstaActivoDebeDevolverFalseCuandoHayEgreso()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        var visitante = new Visitante("Juan", "Pérez", "juan@test.com", "Pass123!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);
        DateTime fechaIngreso = DateTime.Now;

        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, fechaIngreso);
        acceso.RegistrarEgreso(fechaIngreso.AddMinutes(30));

        Assert.IsFalse(acceso.EstaActivo);
    }

    [TestMethod]
    public void TiempoPermanenciaDebeSerNullSinEgreso()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        var visitante = new Visitante("Juan", "Pérez", "juan@test.com", "Pass123!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);

        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now);

        Assert.IsNull(acceso.TiempoPermanencia);
    }

    [TestMethod]
    public void TiempoPermanenciaDebeCalcularDiferenciaConEgreso()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        var visitante = new Visitante("Juan", "Pérez", "juan@test.com", "Pass123!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);
        var fechaIngreso = new DateTime(2025, 9, 30, 14, 0, 0);
        var fechaEgreso = new DateTime(2025, 9, 30, 14, 30, 0);

        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, fechaIngreso);
        acceso.RegistrarEgreso(fechaEgreso);

        Assert.IsNotNull(acceso.TiempoPermanencia);
        Assert.AreEqual(TimeSpan.FromMinutes(30), acceso.TiempoPermanencia.Value);
    }
}
