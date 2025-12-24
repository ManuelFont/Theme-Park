using Dominio.Entities;
using Dominio.Entities.Usuarios;

namespace Test.Domain;

[TestClass]
public class TestRecompensa
{
    [TestMethod]
    public void Constructor_DatosValidosConNivelMembresia_RecompensaCreada()
    {
        var recompensa = new Recompensa("TV", "descripcion", 100, 10, NivelMembresia.Estandar);
        Assert.IsNotNull(recompensa);
        Assert.AreEqual("TV", recompensa.Nombre);
        Assert.AreEqual("descripcion", recompensa.Descripcion);
        Assert.AreEqual(100, recompensa.Costo);
        Assert.AreEqual(10, recompensa.CantidadDisponible);
        Assert.AreEqual(NivelMembresia.Estandar, recompensa.NivelRequerido);
    }

    [TestMethod]
    public void Constructor_DatosValidosSinNivelMembresia_RecompensaCreada()
    {
        var recompensa = new Recompensa("TV", "descripcion", 100, 10);
        Assert.IsNotNull(recompensa);
        Assert.AreEqual("TV", recompensa.Nombre);
        Assert.AreEqual("descripcion", recompensa.Descripcion);
        Assert.AreEqual(100, recompensa.Costo);
        Assert.AreEqual(10, recompensa.CantidadDisponible);
        Assert.IsNull(recompensa.NivelRequerido);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_NombreVacio_ArgumentException()
    {
        _ = new Recompensa("  ", "descripcion", 100, 10);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_DescripcionVacio_ArgumentException()
    {
        _ = new Recompensa("TV", "  ", 100, 10);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_CostoMenorACero_ArgumentException()
    {
        _ = new Recompensa("TV", "descripcion", -1, 10);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_CantidadMenorACero_ArgumentException()
    {
        _ = new Recompensa("TV", "descripcion", 100, -1);
    }
}
