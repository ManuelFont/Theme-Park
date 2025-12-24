using Dominio.Entities;
using Dominio.Entities.Puntuacion;
using ParqueTematico.Application.Plugins;
using ParqueTematico.BusinessLogicInterface;

namespace Test.Application.Plugins;

[TestClass]
public class PluginManagerTests
{
    private PluginManager _manager = null!;
    private MockPluginLoader _mockLoader = null!;

    [TestInitialize]
    public void SetUp()
    {
        _mockLoader = new MockPluginLoader();
        _manager = new PluginManager(_mockLoader);
    }

    [TestMethod]
    public void Inicializar_DeberiaCargarEstrategiasDesdePluginLoader()
    {
        var rutaPlugins = "./TestPlugins";

        _manager.Inicializar(rutaPlugins);

        Assert.IsTrue(_mockLoader.FueLlamadoCargarEstrategias);
        Assert.AreEqual(rutaPlugins, _mockLoader.RutaRecibida);
    }

    [TestMethod]
    public void ObtenerNombresDeEstrategias_DeberiaRetornarNombresDeClases()
    {
        _manager.Inicializar("./TestPlugins");

        var nombres = _manager.ObtenerNombresDeEstrategias();

        Assert.IsTrue(nombres.Any());
        CollectionAssert.Contains(nombres.ToList(), "MockEstrategia1");
        CollectionAssert.Contains(nombres.ToList(), "MockEstrategia2");
    }

    [TestMethod]
    public void CrearInstancia_ConNombreValido_DeberiaRetornarInstanciaDeIPuntuacion()
    {
        _manager.Inicializar("./TestPlugins");

        var estrategia = _manager.CrearInstancia("MockEstrategia1");

        Assert.IsNotNull(estrategia);
        Assert.IsInstanceOfType(estrategia, typeof(IPuntuacion));
    }

    [TestMethod]
    public void CrearInstancia_ConNombreInvalido_DeberiaDevolverNull()
    {
        _manager.Inicializar("./TestPlugins");

        var estrategia = _manager.CrearInstancia("EstrategiaInexistente");

        Assert.IsNull(estrategia);
    }

    [TestMethod]
    public void ObtenerNombresDeEstrategias_SinInicializar_DeberiaRetornarListaVacia()
    {
        var nombres = _manager.ObtenerNombresDeEstrategias();

        Assert.IsFalse(nombres.Any());
    }
}

public class MockPluginLoader : IPluginLoader
{
    public bool FueLlamadoCargarEstrategias { get; private set; }
    public string? RutaRecibida { get; private set; }

    public IEnumerable<Type> CargarEstrategiasDesdePlugins(string rutaCarpeta)
    {
        FueLlamadoCargarEstrategias = true;
        RutaRecibida = rutaCarpeta;

        return [typeof(MockEstrategia1), typeof(MockEstrategia2)];
    }

    public bool ValidarEstrategia(Type tipo)
    {
        return typeof(IPuntuacion).IsAssignableFrom(tipo);
    }
}

public class MockEstrategia1 : IPuntuacion
{
    public string Nombre => "Mock 1";
    public string Descripcion => "Test";

    public int CalcularPuntos(AccesoAtraccion acceso, IEnumerable<AccesoAtraccion> accesosDelDia)
    {
        return 100;
    }
}

public class MockEstrategia2 : IPuntuacion
{
    public string Nombre => "Mock 2";
    public string Descripcion => "Test";

    public int CalcularPuntos(AccesoAtraccion acceso, IEnumerable<AccesoAtraccion> accesosDelDia)
    {
        return 200;
    }
}
