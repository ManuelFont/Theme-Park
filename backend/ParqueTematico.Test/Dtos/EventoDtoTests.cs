using Dominio.Entities;

namespace Test.Dtos;

[TestClass]
public class EventoDtoTests
{
    [TestMethod]
    public void FromEntity_ConEventoValido_DeberiaMapearCorrectamente()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        var evento = new Evento("Noche de Dinosaurios", new DateTime(2026, 10, 15), new TimeSpan(20, 0, 0), 100, 500);
        evento.Atracciones.Add(atraccion);

        var dto = EventoDto.FromEntity(evento);

        Assert.AreEqual(evento.Id, dto.Id);
        Assert.AreEqual("Noche de Dinosaurios", dto.Nombre);
        Assert.AreEqual(new DateTime(2026, 10, 15), dto.Fecha);
        Assert.AreEqual(new TimeSpan(20, 0, 0), dto.Hora);
        Assert.AreEqual(100, dto.Aforo);
        Assert.AreEqual(500, dto.CostoAdicional);
        Assert.AreEqual(1, dto.Atracciones.Count);
        Assert.AreEqual("Montaña Rusa", dto.Atracciones[0].Nombre);
    }

    [TestMethod]
    public void SettersYGetters_DeberianFuncionar()
    {
        var dto = new EventoDto
        {
            Id = Guid.NewGuid(),
            Nombre = "Evento Test",
            Fecha = new DateTime(2025, 12, 25),
            Hora = new TimeSpan(18, 30, 0),
            Aforo = 50,
            CostoAdicional = 250,
            Atracciones = []
        };

        Assert.IsNotNull(dto.Id);
        Assert.AreEqual("Evento Test", dto.Nombre);
        Assert.AreEqual(new DateTime(2025, 12, 25), dto.Fecha);
        Assert.AreEqual(new TimeSpan(18, 30, 0), dto.Hora);
        Assert.AreEqual(50, dto.Aforo);
        Assert.AreEqual(250, dto.CostoAdicional);
        Assert.IsNotNull(dto.Atracciones);
        Assert.AreEqual(0, dto.Atracciones.Count);
    }
}
