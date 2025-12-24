using Dominio.Entities;
using Dominio.Entities.Usuarios;

namespace Test.Domain;

[TestClass]
public class TestCanje
{
    [TestMethod]
    public void Canje_CreadoCorrectamente()
    {
        var usuario = new Operador("Manuel", "Font", "manuel21@mail.com", "Manu2121!");
        var recompensa = new Recompensa("Tele", "des", 200, 20, NivelMembresia.Estandar);
        var canje = new Canje(usuario, recompensa, DateTime.Today);

        Assert.IsNotNull(canje.Id);
        Assert.AreEqual(usuario, canje.Usuario);
        Assert.AreEqual(recompensa, canje.Recompensa);
        Assert.AreEqual(DateTime.Today, canje.FechaCanje);
    }
}
