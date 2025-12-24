using Dominio.Entities.Puntuacion;

namespace Test.Domain;

[TestClass]
public class TestRanking
{
    private Ranking? _ranking;

    [TestMethod]
    public void Ranking_PuntuacionPorAtraccion_EstrategiaActivaCorrecta()
    {
        var puntuacionPorAtraccion = new PuntuacionPorAtraccion();
        _ranking = new Ranking(puntuacionPorAtraccion);
        Assert.AreEqual(_ranking.EstrategiaActiva, puntuacionPorAtraccion);
    }
}
