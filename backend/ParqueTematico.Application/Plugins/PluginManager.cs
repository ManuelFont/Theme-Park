using Dominio.Entities.Puntuacion;
using ParqueTematico.BusinessLogicInterface;

namespace ParqueTematico.Application.Plugins;

public class PluginManager(IPluginLoader pluginLoader) : IPluginManager
{
    private readonly Dictionary<string, Type> _estrategias = [];
    private readonly IPluginLoader _pluginLoader = pluginLoader;

    public void Inicializar(string rutaCarpetaPlugins)
    {
        _estrategias.Clear();

        var tipos = _pluginLoader.CargarEstrategiasDesdePlugins(rutaCarpetaPlugins);

        foreach(var tipo in tipos)
        {
            _estrategias[tipo.Name] = tipo;
        }
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
