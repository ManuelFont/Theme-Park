using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Dtos;

namespace Test.Dtos;

[TestClass]
public class TicketDtoTests
{
    private readonly DateTime fecha = new DateTime(2000, 1, 1);

    [TestMethod]
    public void FromEntity_ConTicketGeneral_DeberiaMapearCorrectamente()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", fecha, NivelMembresia.Estandar);
        var fechaVisita = new DateTime(2025, 10, 15);
        var ticket = new Ticket(visitante, fechaVisita, TipoEntrada.General, null);

        var dto = TicketDto.FromEntity(ticket);

        Assert.AreEqual(ticket.Id, dto.Id);
        Assert.AreEqual(visitante.Id, dto.Visitante.Id);
        Assert.AreEqual("Juan", dto.Visitante.Nombre);
        Assert.AreEqual("Perez", dto.Visitante.Apellido);
        Assert.AreEqual("juan@test.com", dto.Visitante.Email);
        Assert.AreEqual(fechaVisita, dto.FechaVisita);
        Assert.AreEqual("General", dto.TipoEntrada);
        Assert.IsNull(dto.EventoAsociado);
    }

    [TestMethod]
    public void FromEntity_ConTicketEventoEspecial_DeberiaMapearCorrectamente()
    {
        var visitante = new Visitante("Maria", "Lopez", "maria@test.com", "Pass1234!", new DateTime(1995, 5, 10), NivelMembresia.Premium);
        var evento = new Evento("Noche de Dinosaurios", new DateTime(2026, 10, 31), new TimeSpan(20, 0, 0), 100, 500);
        var fechaVisita = new DateTime(2026, 10, 31);
        var ticket = new Ticket(visitante, fechaVisita, TipoEntrada.EventoEspecial, evento);

        var dto = TicketDto.FromEntity(ticket);

        Assert.AreEqual(ticket.Id, dto.Id);
        Assert.AreEqual(visitante.Id, dto.Visitante.Id);
        Assert.AreEqual("Maria", dto.Visitante.Nombre);
        Assert.AreEqual("Lopez", dto.Visitante.Apellido);
        Assert.AreEqual("maria@test.com", dto.Visitante.Email);
        Assert.AreEqual(fechaVisita, dto.FechaVisita);
        Assert.AreEqual("EventoEspecial", dto.TipoEntrada);
        Assert.IsNotNull(dto.EventoAsociado);
        Assert.AreEqual(evento.Id, dto.EventoAsociado.Id);
        Assert.AreEqual("Noche de Dinosaurios", dto.EventoAsociado.Nombre);
    }

    [TestMethod]
    public void SettersYGetters_DeberianFuncionar()
    {
        var visitanteDto = new UsuarioDTO(Guid.NewGuid(), "Test", "User", "test@test.com", "Visitante", fecha, nameof(NivelMembresia.Estandar), 0);
        var dto = new TicketDto
        {
            Id = Guid.NewGuid(),
            Visitante = visitanteDto,
            FechaVisita = new DateTime(2025, 12, 25),
            TipoEntrada = "General",
            EventoAsociado = null
        };

        Assert.IsNotNull(dto.Id);
        Assert.AreEqual(visitanteDto.Id, dto.Visitante.Id);
        Assert.AreEqual("Test", dto.Visitante.Nombre);
        Assert.AreEqual("User", dto.Visitante.Apellido);
        Assert.AreEqual("test@test.com", dto.Visitante.Email);
        Assert.AreEqual(new DateTime(2025, 12, 25), dto.FechaVisita);
        Assert.AreEqual("General", dto.TipoEntrada);
        Assert.IsNull(dto.EventoAsociado);
    }
}
