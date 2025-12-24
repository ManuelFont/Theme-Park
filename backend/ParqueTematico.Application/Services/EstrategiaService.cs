using Dominio.Entities.Puntuacion;
using ParqueTematico.BusinessLogicInterface;

namespace ParqueTematico.Application.Services;

public class EstrategiaService(IPluginManager pluginManager) : IEstrategiaService
{
    private readonly IPluginManager _pluginManager = pluginManager;
    private IPuntuacion _estrategiaActiva = new PuntuacionPorAtraccion();

    public IPuntuacion ObtenerEstrategiaActiva()
    {
        return _estrategiaActiva;
    }

    public bool CambiarEstrategia(string nombreEstrategia)
    {
        var nuevaEstrategia = _pluginManager.CrearInstancia(nombreEstrategia);

        if(nuevaEstrategia == null)
        {
            return false;
        }

        _estrategiaActiva = nuevaEstrategia;
        return true;
    }

    public IEnumerable<(string Nombre, string Descripcion)> ObtenerEstrategiasDisponibles()
    {
        foreach(var nombre in _pluginManager.ObtenerNombresDeEstrategias())
        {
            var instancia = _pluginManager.CrearInstancia(nombre);
            if(instancia != null)
            {
                yield return (nombre, instancia.Descripcion);
            }
        }
    }
}
