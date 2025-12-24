using Microsoft.Extensions.DependencyInjection;
using ParqueTematico.Application.Plugins;
using ParqueTematico.BusinessLogicInterface;

namespace Test.Application.Plugins;

[TestClass]
public class PluginServiceExtensionsTests
{
    private ServiceCollection _services = null!;

    [TestInitialize]
    public void SetUp()
    {
        _services = new ServiceCollection();
    }

    [TestMethod]
    public void AddPluginSystem_DeberiaRegistrarPluginLoader()
    {
        _services.AddPluginSystem();

        var serviceProvider = _services.BuildServiceProvider();
        var pluginLoader = serviceProvider.GetService<IPluginLoader>();

        Assert.IsNotNull(pluginLoader);
        Assert.IsInstanceOfType<PluginLoader>(pluginLoader);
    }

    [TestMethod]
    public void AddPluginSystem_DeberiaRegistrarPluginManager()
    {
        _services.AddPluginSystem();

        var serviceProvider = _services.BuildServiceProvider();
        var pluginManager = serviceProvider.GetService<IPluginManager>();

        Assert.IsNotNull(pluginManager);
        Assert.IsInstanceOfType<PluginManager>(pluginManager);
    }

    [TestMethod]
    public void AddPluginSystem_DeberiaRegistrarEstrategiaService()
    {
        _services.AddPluginSystem();

        var serviceProvider = _services.BuildServiceProvider();
        var estrategiaService = serviceProvider.GetService<IEstrategiaService>();

        Assert.IsNotNull(estrategiaService);
    }

    [TestMethod]
    public void AddPluginSystem_PluginLoaderDeberiaSingleton()
    {
        _services.AddPluginSystem();

        var serviceProvider = _services.BuildServiceProvider();
        var instance1 = serviceProvider.GetService<IPluginLoader>();
        var instance2 = serviceProvider.GetService<IPluginLoader>();

        Assert.AreSame(instance1, instance2);
    }

    [TestMethod]
    public void AddPluginSystem_PluginManagerDeberiaSingleton()
    {
        _services.AddPluginSystem();

        var serviceProvider = _services.BuildServiceProvider();
        var instance1 = serviceProvider.GetService<IPluginManager>();
        var instance2 = serviceProvider.GetService<IPluginManager>();

        Assert.AreSame(instance1, instance2);
    }

    [TestMethod]
    public void InicializarPluginSystem_CarpetaNoExiste_NoDeberiaLanzarExcepcion()
    {
        _services.AddPluginSystem();
        var serviceProvider = _services.BuildServiceProvider();

        serviceProvider.InicializarPluginSystem("C:\\carpeta\\que\\no\\existe");

        var pluginManager = serviceProvider.GetService<IPluginManager>();
        Assert.IsNotNull(pluginManager);
    }

    [TestMethod]
    public void InicializarPluginSystem_DeberiaInicializarPluginManager()
    {
        _services.AddPluginSystem();
        var serviceProvider = _services.BuildServiceProvider();
        var carpetaTemporal = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(carpetaTemporal);

        var dllOrigen = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ParqueTematico.Domain.dll");
        var dllDestino = Path.Combine(carpetaTemporal, "ParqueTematico.Domain.dll");
        File.Copy(dllOrigen, dllDestino);

        serviceProvider.InicializarPluginSystem(carpetaTemporal);

        var pluginManager = serviceProvider.GetRequiredService<IPluginManager>();
        var estrategias = pluginManager.ObtenerNombresDeEstrategias();

        Assert.IsTrue(estrategias.Any());
        Directory.Delete(carpetaTemporal, true);
    }
}
