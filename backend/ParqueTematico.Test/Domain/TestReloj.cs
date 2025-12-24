using Dominio.Entities;

namespace Test.Domain;

[TestClass]
public class TestReloj
{
    private Reloj? _reloj;

    [TestMethod]
    public void Calculadora_SinParametros_FechaHoraActual()
    {
        _reloj = new Reloj();
        var dif = (DateTime.Now - _reloj.FechaHora).TotalSeconds;
        Assert.IsTrue(dif < 1);
    }
}
