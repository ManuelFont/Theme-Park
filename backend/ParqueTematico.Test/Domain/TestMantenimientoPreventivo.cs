using Dominio.Entities;
using Dominio.Exceptions;

namespace Test.Domain;

[TestClass]
public class TestMantenimientoPreventivo
{
    private Atraccion atraccion = null!;

    [TestInitialize]
    public void SetUp()
    {
        atraccion = new Atraccion("montana loca", TipoAtraccion.MontañaRusa, 12, 10, "des", true);
    }

    [TestMethod]
    public void Constructor_ValoresCorrectos_IncidenciaCreada()
    {
        DateTime maniana = DateTime.Today.AddDays(1);
        var mantenimiento = new MantenimientoPreventivo(atraccion, "des", true, DateTime.Today, maniana);
        Assert.IsNotNull(mantenimiento);
    }

    [TestMethod]
    [ExpectedException(typeof(IncidenciaException))]
    public void Constructor_FechaFinMenorAFechaInicio_LanzarError()
    {
        DateTime ayer = DateTime.Today.AddDays(-1);
        var mantenimiento = new MantenimientoPreventivo(atraccion, "des", true, DateTime.Today, ayer);
        Assert.IsNotNull(mantenimiento);
    }
}
