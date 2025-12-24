using Dominio.Entities;
using Dominio.Exceptions;

namespace Test.Domain;

[TestClass]
public class TestIncidencia
{
    private Atraccion CrearAtraccionDummy()
    {
        return new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 10, 50, "Montaña rusa leve", true);
    }

    [TestMethod]
    public void Constructor_Valido_DeberiaCrearIncidencia()
    {
        Atraccion atraccion = CrearAtraccionDummy();
        var incidencia = new Incidencia(atraccion, TipoIncidencia.FueraDeServicio,
            "Esta atracción se encuentra fuera de servicio", false);

        Assert.IsNotNull(incidencia.Id);
        Assert.AreEqual(atraccion, incidencia.Atraccion);
        Assert.AreEqual(TipoIncidencia.FueraDeServicio, incidencia.TipoIncidencia);
        Assert.AreEqual(incidencia.Descripcion, "Esta atracción se encuentra fuera de servicio");
        Assert.IsFalse(incidencia.EstaActiva);
    }

    [TestMethod]
    [ExpectedException(typeof(IncidenciaException))]
    public void ConstructorAtraccionNuloDeberiaDarExcepcion()
    {
        new Incidencia(null!, TipoIncidencia.FueraDeServicio,
            "Esta atracción se encuentra fuera de servicio", false);
    }

    [TestMethod]
    [ExpectedException(typeof(IncidenciaException))]
    public void ConstruccionDescripcionNulaDeberiaDarExcepcion()
    {
        new Incidencia(null!, TipoIncidencia.FueraDeServicio,
            "Esta atracción se encuentra fuera de servicio", false);
    }

    [TestMethod]
    public void Activar_CambiaEstadoAEstaActiva()
    {
        Atraccion atraccion = CrearAtraccionDummy();
        var incidencia = new Incidencia(atraccion, TipoIncidencia.Mantenimiento,
            "La atracción se encuentra en mantenimiento.", false);

        incidencia.Activar();

        Assert.IsTrue(incidencia.EstaActiva);
    }

    [TestMethod]
    public void Desactivar_CambiaEstadoAEstaInactiva()
    {
        Atraccion atraccion = CrearAtraccionDummy();
        var incidencia = new Incidencia(atraccion, TipoIncidencia.Mantenimiento,
            "La atracción se encuentra en mantenimiento.", true);

        incidencia.Desactivar();

        Assert.IsFalse(incidencia.EstaActiva);
    }
}
