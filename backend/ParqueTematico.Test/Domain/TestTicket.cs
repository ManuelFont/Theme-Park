using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Dominio.Exceptions;

namespace Test.Domain;

[TestClass]
public class TicketTests
{
    [TestMethod]
    public void Ticket_General_Valido_DeberiaCrearseCorrectamente()
    {
        var visitante = new Visitante(
            "Juan", "Perez",
            "nahuelmileo@hotmail.com",
            "Abcdef1!",
            new DateTime(2025, 1, 1),
            NivelMembresia.Estandar);

        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);

        Assert.AreEqual(visitante, ticket.Visitante);
        Assert.AreEqual(TipoEntrada.General, ticket.TipoEntrada);
        Assert.IsNull(ticket.EventoAsociado);
        Assert.AreNotEqual(Guid.Empty, ticket.Id);
    }

    [TestMethod]
    public void Ticket_EventoEspecial_Valido_DeberiaCrearseCorrectamente()
    {
        var visitante = new Visitante(
            "Juan", "Perez",
            "nahuelmileo@hotmail.com",
            "Abcde!f1",
            new DateTime(2025, 1, 1),
            NivelMembresia.Estandar);

        var evento = new Evento("Evento de prueba", DateTime.Now.AddDays(2), new TimeSpan(18, 0, 0), 100, 50);

        var ticket = new Ticket(visitante, DateTime.Now.AddDays(2), TipoEntrada.EventoEspecial, evento);

        Assert.AreEqual(visitante, ticket.Visitante);
        Assert.AreEqual(TipoEntrada.EventoEspecial, ticket.TipoEntrada);
        Assert.AreEqual(evento, ticket.EventoAsociado);
        Assert.AreNotEqual(Guid.Empty, ticket.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(TicketException))]
    public void Ticket_EventoEspecial_SinEvento_DeberiaLanzarExcepcion()
    {
        var visitante = new Visitante(
            "Juan", "Perez",
            "nahuelmileo@hotmail.com",
            "Ab!cdef1",
            new DateTime(2025, 1, 1),
            NivelMembresia.Estandar);

        _ = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.EventoEspecial, null);
    }

    [TestMethod]
    [ExpectedException(typeof(TicketException))]
    public void Ticket_VisitanteNulo_DeberiaLanzarExcepcion()
    {
        _ = new Ticket(null!, DateTime.Now.AddDays(1), TipoEntrada.General, null);
    }

    [TestMethod]
    public void CambiarFechaVisita_Valida_DeberiaActualizarFecha()
    {
        var visitante = new Visitante(
            "Juan", "Perez",
            "juancito@gmail.com",
            "Abcdef1!",
            new DateTime(2025, 1, 1),
            NivelMembresia.Estandar);

        var ticket = new Ticket(visitante, DateTime.Now.AddDays(2), TipoEntrada.General, null);

        DateTime nuevaFecha = DateTime.Now.AddDays(10);
        ticket.CambiarFechaVisita(nuevaFecha);

        Assert.AreEqual(nuevaFecha.Date, ticket.FechaVisita.Date);
        Assert.AreEqual(TipoEntrada.General, ticket.TipoEntrada);
        Assert.IsNull(ticket.EventoAsociado);
    }

    [TestMethod]
    [ExpectedException(typeof(TicketException))]
    public void CambiarFechaVisita_Pasada_DeberiaLanzarExcepcion()
    {
        var visitante = new Visitante(
            "Juan", "Perez",
            "juancito@gmail.com",
            "Abcdef1!",
            new DateTime(2025, 1, 1),
            NivelMembresia.Estandar);

        var ticket = new Ticket(visitante, DateTime.Now.AddDays(2), TipoEntrada.General, null);
        DateTime fechaPasada = DateTime.Now.AddDays(-1);

        ticket.CambiarFechaVisita(fechaPasada);
    }

    [TestMethod]
    public void EsValidoParaFecha_FechaCoincide_DeberiaRetornarTrue()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var fechaVisita = new DateTime(2025, 10, 15);
        var ticket = new Ticket(visitante, fechaVisita, TipoEntrada.General, null);

        var resultado = ticket.EsValidoParaFecha(new DateTime(2025, 10, 15, 14, 30, 0));

        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void EsValidoParaFecha_FechaDiferente_DeberiaRetornarFalse()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var fechaVisita = new DateTime(2025, 10, 15);
        var ticket = new Ticket(visitante, fechaVisita, TipoEntrada.General, null);

        var resultado = ticket.EsValidoParaFecha(new DateTime(2025, 10, 16));

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void IncluyeAtraccion_TicketGeneral_DeberiaRetornarTrue()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(1), TipoEntrada.General, null);

        var resultado = ticket.IncluyeAtraccion(Guid.NewGuid());

        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void IncluyeAtraccion_EventoEspecialSinAtracciones_DeberiaRetornarFalse()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var evento = new Evento("Festival", DateTime.Now.AddDays(2), new TimeSpan(18, 0, 0), 100, 500);
        var ticket = new Ticket(visitante, DateTime.Now.AddDays(2), TipoEntrada.EventoEspecial, evento);

        var resultado = ticket.IncluyeAtraccion(Guid.NewGuid());

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void IncluyeAtraccion_EventoEspecialConAtraccion_DeberiaRetornarTrue()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var evento = new Evento("Festival", DateTime.Now.AddDays(2), new TimeSpan(18, 0, 0), 100, 500);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripción", true);
        evento.AgregarAtraccion(atraccion);

        var ticket = new Ticket(visitante, DateTime.Now.AddDays(2), TipoEntrada.EventoEspecial, evento);

        var resultado = ticket.IncluyeAtraccion(atraccion.Id);

        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void IncluyeAtraccion_EventoEspecialConOtraAtraccion_DeberiaRetornarFalse()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var evento = new Evento("Festival", DateTime.Now.AddDays(2), new TimeSpan(18, 0, 0), 100, 500);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripción", true);
        evento.AgregarAtraccion(atraccion);

        var ticket = new Ticket(visitante, DateTime.Now.AddDays(2), TipoEntrada.EventoEspecial, evento);

        var resultado = ticket.IncluyeAtraccion(Guid.NewGuid());

        Assert.IsFalse(resultado);
    }
}
