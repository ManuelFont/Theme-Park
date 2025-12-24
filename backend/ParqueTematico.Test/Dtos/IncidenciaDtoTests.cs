using Dominio.Entities;
using Dtos;

namespace Test.Dtos;

[TestClass]
public class IncidenciaDtoTests
{
    [TestMethod]
    public void FromEntity_ConIncidenciaValida_DeberiaMapearCorrectamente()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);
        var incidencia = new Incidencia(atraccion, TipoIncidencia.Mantenimiento, "Cambio de correas", true);

        var dto = IncidenciaDto.FromEntity(incidencia);

        Assert.AreEqual(incidencia.Id, dto.Id);
        Assert.AreEqual(atraccion.Id, dto.AtraccionId);
        Assert.AreEqual(TipoIncidencia.Mantenimiento, dto.TipoIncidencia);
        Assert.AreEqual("Cambio de correas", dto.Descripcion);
        Assert.IsTrue(dto.EstaActiva);
    }

    [TestMethod]
    public void ToEntity_ConDtoValido_DeberiaCrearIncidencia()
    {
        var atraccion = new Atraccion("Espectáculo Musical", TipoAtraccion.Espectáculo, 8, 20, "Show en vivo", true);

        var dto = new IncidenciaDto
        {
            Id = Guid.NewGuid(),
            AtraccionId = atraccion.Id,
            TipoIncidencia = TipoIncidencia.Rota,
            Descripcion = "Falla eléctrica",
            EstaActiva = false
        };

        Incidencia incidencia = dto.ToEntity(atraccion);

        Assert.AreEqual(dto.Id, incidencia.Id);
        Assert.AreEqual(atraccion.Id, incidencia.Atraccion.Id);
        Assert.AreEqual("Espectáculo Musical", incidencia.Atraccion.Nombre);
        Assert.AreEqual(TipoIncidencia.Rota, incidencia.TipoIncidencia);
        Assert.AreEqual("Falla eléctrica", incidencia.Descripcion);
        Assert.IsFalse(incidencia.EstaActiva);
    }

    [TestMethod]
    public void SettersYGetters_DeberianFuncionar()
    {
        var atraccion = new Atraccion("Test", TipoAtraccion.MontañaRusa, 10, 25, "Desc", true);

        var dto = new IncidenciaDto
        {
            Id = Guid.NewGuid(),
            AtraccionId = atraccion.Id,
            TipoIncidencia = TipoIncidencia.Mantenimiento,
            Descripcion = "Test desc",
            EstaActiva = true
        };

        Assert.IsNotNull(dto.Id);
        Assert.AreEqual(TipoIncidencia.Mantenimiento, dto.TipoIncidencia);
        Assert.AreEqual("Test desc", dto.Descripcion);
        Assert.IsTrue(dto.EstaActiva);
    }
}
