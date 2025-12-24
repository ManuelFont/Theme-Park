using Dominio.Entities.Puntuacion;

namespace ParqueTematico.BusinessLogicInterface;

public interface IPluginManager
{
    void Inicializar(string rutaCarpetaPlugins);
    IEnumerable<string> ObtenerNombresDeEstrategias();
    IPuntuacion? CrearInstancia(string nombreEstrategia);
}
