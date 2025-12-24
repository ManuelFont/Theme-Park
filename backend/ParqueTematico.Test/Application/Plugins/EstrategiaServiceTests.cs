using Dominio.Entities.Puntuacion;
using ParqueTematico.Application.Services;
using ParqueTematico.BusinessLogicInterface;

namespace Test.Application.Plugins;

[TestClass]
public class EstrategiaServiceTests
{
    [TestMethod]
    public void ObtenerEstrategiaActiva_SinCambios_DeberiaRetornarEstrategiaPorDefecto()
    {
        var mockPluginManager = new MockPluginManager();
        var service = new EstrategiaService(mockPluginManager);

        var estrategia = service.ObtenerEstrategiaActiva();

        Assert.IsNotNull(estrategia);
        Assert.IsInstanceOfType(estrategia, typeof(IPuntuacion));
    }

    [TestMethod]
    public void CambiarEstrategia_ConNombreValido_DeberiaCambiarEstrategiaActiva()
    {
        var mockPluginManager = new MockPluginManager();
        var service = new EstrategiaService(mockPluginManager);

        var resultado = service.CambiarEstrategia("MockEstrategia1");

        Assert.IsTrue(resultado);
        var estrategiaActiva = service.ObtenerEstrategiaActiva();
        Assert.AreEqual("Mock 1", estrategiaActiva.Nombre);
    }
}

public class MockPluginManager : IPluginManager
{
    private readonly Dictionary<string, Type> _estrategias = new()
    {
        { "MockEstrategia1", typeof(MockEstrategia1) },
        { "MockEstrategia2", typeof(MockEstrategia2) }
    };

    public void Inicializar(string rutaCarpetaPlugins)
    {
    }

    public IEnumerable<string> ObtenerNombresDeEstrategias()
    {
        return _estrategias.Keys;
    }

    public IPuntuacion? CrearInstancia(string nombreEstrategia)
    {
        if(!_estrategias.ContainsKey(nombreEstrategia))
        {
            return null;
        }

        var tipo = _estrategias[nombreEstrategia];
        return Activator.CreateInstance(tipo) as IPuntuacion;
    }
}
