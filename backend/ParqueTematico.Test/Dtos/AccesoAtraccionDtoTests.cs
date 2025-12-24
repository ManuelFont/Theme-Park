using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Dtos;

namespace Test.Dtos;

[TestClass]
public class AccesoAtraccionDtoTests
{
    private DateTime? fecha = null;
    [TestInitialize]
    public void Setup()
    {
        fecha = new DateTime(2000, 1, 1);
    }

    [TestMethod]
    public void FromEntity_DeberiaConvertirCorrectamente()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);
        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, DateTime.Now);

        var dto = AccesoAtraccionDto.FromEntity(acceso);

        Assert.AreEqual(acceso.Id, dto.Id);
        Assert.AreEqual(visitante.Id, dto.Visitante.Id);
        Assert.AreEqual(atraccion.Id, dto.Atraccion.Id);
        Assert.AreEqual(ticket.Id, dto.TicketId);
        Assert.AreEqual(acceso.FechaHoraIngreso, dto.FechaHoraIngreso);
        Assert.IsNull(dto.FechaHoraEgreso);
        Assert.AreEqual(0, dto.PuntosObtenidos);
    }

    [TestMethod]
    public void ToEntity_DeberiaConvertirCorrectamente()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);

        var dto = new AccesoAtraccionDto
        {
            Id = Guid.NewGuid(),
            Visitante = new UsuarioDTO(visitante.Id, "Juan", "Perez", "juan@test.com", "Visitante", fecha, nameof(NivelMembresia.Estandar), 0),
            Atraccion = AtraccionDto.FromEntity(atraccion),
            TicketId = ticket.Id,
            FechaHoraIngreso = DateTime.Now,
            FechaHoraEgreso = null,
            PuntosObtenidos = 0
        };

        AccesoAtraccion acceso = dto.ToEntity(visitante, atraccion, ticket);

        Assert.IsNotNull(acceso);
        Assert.AreEqual(visitante.Id, acceso.Visitante.Id);
        Assert.AreEqual(atraccion.Id, acceso.Atraccion.Id);
        Assert.AreEqual(ticket.Id, acceso.Ticket.Id);
        Assert.IsNull(acceso.FechaHoraEgreso);
    }

    [TestMethod]
    public void ToEntity_ConFechaEgreso_DeberiaRegistrarEgreso()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);
        DateTime fechaEgreso = DateTime.Now.AddHours(1);

        var dto = new AccesoAtraccionDto
        {
            Id = Guid.NewGuid(),
            Visitante = new UsuarioDTO(visitante.Id, "Juan", "Perez", "juan@test.com", "Visitante", fecha, nameof(NivelMembresia.Estandar), 0),
            Atraccion = AtraccionDto.FromEntity(atraccion),
            TicketId = ticket.Id,
            FechaHoraIngreso = DateTime.Now,
            FechaHoraEgreso = fechaEgreso,
            PuntosObtenidos = 0
        };

        AccesoAtraccion acceso = dto.ToEntity(visitante, atraccion, ticket);

        Assert.IsNotNull(acceso.FechaHoraEgreso);
        Assert.AreEqual(fechaEgreso, acceso.FechaHoraEgreso);
    }

    [TestMethod]
    public void ToEntity_ConPuntosObtenidos_DeberiaAsignarPuntos()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);

        var dto = new AccesoAtraccionDto
        {
            Id = Guid.NewGuid(),
            Visitante = new UsuarioDTO(visitante.Id, "Juan", "Perez", "juan@test.com", "Visitante", fecha, nameof(NivelMembresia.Estandar), 0),
            Atraccion = AtraccionDto.FromEntity(atraccion),
            TicketId = ticket.Id,
            FechaHoraIngreso = DateTime.Now,
            FechaHoraEgreso = null,
            PuntosObtenidos = 150
        };

        AccesoAtraccion acceso = dto.ToEntity(visitante, atraccion, ticket);

        Assert.AreEqual(150, acceso.PuntosObtenidos);
    }

    [TestMethod]
    public void ToEntity_ConFechaEgresoYPuntos_DeberiaAsignarAmbos()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        var ticket = new Ticket(visitante, DateTime.Now, TipoEntrada.General, null);
        DateTime fechaEgreso = DateTime.Now.AddHours(1);

        var dto = new AccesoAtraccionDto
        {
            Id = Guid.NewGuid(),
            Visitante = new UsuarioDTO(visitante.Id, "Juan", "Perez", "juan@test.com", "Visitante", fecha, nameof(NivelMembresia.Estandar), 0),
            Atraccion = AtraccionDto.FromEntity(atraccion),
            TicketId = ticket.Id,
            FechaHoraIngreso = DateTime.Now,
            FechaHoraEgreso = fechaEgreso,
            PuntosObtenidos = 200
        };

        AccesoAtraccion acceso = dto.ToEntity(visitante, atraccion, ticket);

        Assert.IsNotNull(acceso.FechaHoraEgreso);
        Assert.AreEqual(fechaEgreso, acceso.FechaHoraEgreso);
        Assert.AreEqual(200, acceso.PuntosObtenidos);
    }
}
