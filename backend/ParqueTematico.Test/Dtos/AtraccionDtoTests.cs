using Dominio.Entities;
using Dtos;

namespace Test.Dtos;

[TestClass]
public class AtraccionDtoTests
{
    [TestMethod]
    public void FromEntity_ConAtraccionValida_DeberiaMapearCorrectamente()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción test", true);

        var dto = AtraccionDto.FromEntity(atraccion);

        Assert.AreEqual(atraccion.Id, dto.Id);
        Assert.AreEqual("Montaña Rusa", dto.Nombre);
        Assert.AreEqual(TipoAtraccion.MontañaRusa, dto.Tipo);
        Assert.AreEqual(12, dto.EdadMinima);
        Assert.AreEqual(30, dto.CapacidadMaxima);
        Assert.AreEqual("Descripción test", dto.Descripcion);
    }

    [TestMethod]
    public void ToEntity_ConDtoValido_DeberiaCrearAtraccion()
    {
        var dto = new AtraccionDto
        {
            Id = Guid.NewGuid(),
            Nombre = "Simulador VR",
            Tipo = TipoAtraccion.Simulador,
            EdadMinima = 8,
            CapacidadMaxima = 20,
            Descripcion = "Experiencia virtual"
        };

        Atraccion atraccion = dto.ToEntity();

        Assert.AreEqual(dto.Id, atraccion.Id);
        Assert.AreEqual("Simulador VR", atraccion.Nombre);
        Assert.AreEqual(TipoAtraccion.Simulador, atraccion.Tipo);
        Assert.AreEqual(8, atraccion.EdadMinima);
        Assert.AreEqual(20, atraccion.CapacidadMaxima);
        Assert.AreEqual("Experiencia virtual", atraccion.Descripcion);
    }

    [TestMethod]
    public void SettersYGetters_DeberianaFuncionar()
    {
        var dto = new AtraccionDto
        {
            Id = Guid.NewGuid(),
            Nombre = "Test",
            Tipo = TipoAtraccion.MontañaRusa,
            EdadMinima = 10,
            CapacidadMaxima = 25,
            Descripcion = "Desc"
        };

        Assert.IsNotNull(dto.Id);
        Assert.AreEqual("Test", dto.Nombre);
        Assert.AreEqual(TipoAtraccion.MontañaRusa, dto.Tipo);
        Assert.AreEqual(10, dto.EdadMinima);
        Assert.AreEqual(25, dto.CapacidadMaxima);
        Assert.AreEqual("Desc", dto.Descripcion);
    }
}
