using Dominio.Entities;
using Dominio.Entities.Puntuacion;
using ParqueTematico.Application.Plugins;

namespace Test.Application.Plugins;

[TestClass]
public class PluginLoaderTests
{
    private PluginLoader _loader = null!;

    [TestInitialize]
    public void SetUp()
    {
        _loader = new PluginLoader();
    }

    [TestMethod]
    public void ValidarEstrategia_ClaseQueImplementaIPuntuacion_DeberiaRetornarTrue()
    {
        var tipo = typeof(PuntuacionPorAtraccion);

        var resultado = _loader.ValidarEstrategia(tipo);

        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void ValidarEstrategia_ClaseAbstracta_DeberiaRetornarFalse()
    {
        var tipo = typeof(EstrategiaAbstracta);

        var resultado = _loader.ValidarEstrategia(tipo);

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void ValidarEstrategia_Interfaz_DeberiaRetornarFalse()
    {
        var tipo = typeof(IPuntuacion);

        var resultado = _loader.ValidarEstrategia(tipo);

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void ValidarEstrategia_ClaseSinConstructorVacio_DeberiaRetornarFalse()
    {
        var tipo = typeof(EstrategiaSinConstructorVacio);

        var resultado = _loader.ValidarEstrategia(tipo);

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void ValidarEstrategia_ClaseQueNoImplementaIPuntuacion_DeberiaRetornarFalse()
    {
        var tipo = typeof(ClaseNormal);

        var resultado = _loader.ValidarEstrategia(tipo);

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void CargarEstrategiasDesdePlugins_CarpetaNoExiste_DeberiaRetornarListaVacia()
    {
        var resultado = _loader.CargarEstrategiasDesdePlugins("C:\\carpeta\\que\\no\\existe");

        Assert.IsFalse(resultado.Any());
    }

    [TestMethod]
    public void CargarEstrategiasDesdePlugins_CarpetaVacia_DeberiaRetornarListaVacia()
    {
        var carpetaTemporal = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(carpetaTemporal);

        var resultado = _loader.CargarEstrategiasDesdePlugins(carpetaTemporal);

        Assert.IsFalse(resultado.Any());
        Directory.Delete(carpetaTemporal);
    }

    [TestMethod]
    public void CargarEstrategiasDesdePlugins_ConDllValida_DeberiaCargaEstrategia()
    {
        var carpetaTemporal = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(carpetaTemporal);

        var dllOrigen = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ParqueTematico.Domain.dll");
        var dllDestino = Path.Combine(carpetaTemporal, "ParqueTematico.Domain.dll");
        File.Copy(dllOrigen, dllDestino);

        var resultado = _loader.CargarEstrategiasDesdePlugins(carpetaTemporal);

        Assert.IsTrue(resultado.Any());
        Assert.IsTrue(resultado.All(t => typeof(IPuntuacion).IsAssignableFrom(t)));

        Directory.Delete(carpetaTemporal, true);
    }

    [TestMethod]
    public void CargarEstrategiasDesdePlugins_ConDllInvalidaYValidaJuntas_DeberiaCargaSoloLasValidas()
    {
        var carpetaTemporal = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(carpetaTemporal);

        var archivoInvalido = Path.Combine(carpetaTemporal, "invalido.dll");
        File.WriteAllText(archivoInvalido, "Este no es un DLL valido");

        var dllOrigen = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ParqueTematico.Domain.dll");
        var dllDestino = Path.Combine(carpetaTemporal, "ParqueTematico.Domain.dll");
        File.Copy(dllOrigen, dllDestino);

        var resultado = _loader.CargarEstrategiasDesdePlugins(carpetaTemporal);

        Assert.IsTrue(resultado.Any());
        Directory.Delete(carpetaTemporal, true);
    }

    [TestMethod]
    public void CargarEstrategiasDesdePlugins_SoloDllsInvalidas_DeberiaLanzarExcepcion()
    {
        var carpetaTemporal = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(carpetaTemporal);

        var archivoInvalido = Path.Combine(carpetaTemporal, "invalido.dll");
        File.WriteAllText(archivoInvalido, "Este no es un DLL valido");

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            _loader.CargarEstrategiasDesdePlugins(carpetaTemporal);
        });

        Directory.Delete(carpetaTemporal, true);
    }
}

public abstract class EstrategiaAbstracta : IPuntuacion
{
    public string Nombre => "Abstracta";
    public string Descripcion => "Test";
    public int CalcularPuntos(AccesoAtraccion acceso, IEnumerable<AccesoAtraccion> accesosDelDia) => 0;
}

public class EstrategiaSinConstructorVacio(int valor) : IPuntuacion
{
    private readonly int _valor = valor;

    public string Nombre => "Sin Constructor Vacio";
    public string Descripcion => "Test";
    public int CalcularPuntos(AccesoAtraccion acceso, IEnumerable<AccesoAtraccion> accesosDelDia) => _valor;
}

public class ClaseNormal
{
    public string Nombre => "No es estrategia";
}
