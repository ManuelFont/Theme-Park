namespace ParqueTematico.BusinessLogicInterface;

public interface IPluginLoader
{
    IEnumerable<Type> CargarEstrategiasDesdePlugins(string rutaCarpeta);
    bool ValidarEstrategia(Type tipo);
}
