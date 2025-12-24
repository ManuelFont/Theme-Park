using System.Reflection;
using Dominio.Entities.Puntuacion;
using ParqueTematico.BusinessLogicInterface;

namespace ParqueTematico.Application.Plugins;

public class PluginLoader : IPluginLoader
{
    public IEnumerable<Type> CargarEstrategiasDesdePlugins(string rutaCarpeta)
    {
        if(!Directory.Exists(rutaCarpeta))
        {
            return Enumerable.Empty<Type>();
        }

        var estrategias = new List<Type>();
        var archivos = Directory.GetFiles(rutaCarpeta, "*.dll");
        var errorCount = 0;

        foreach(var archivo in archivos)
        {
            try
            {
                var assembly = Assembly.LoadFrom(archivo);
                var tipos = assembly.GetTypes();
                var estrategiasEncontradas = tipos.Where(ValidarEstrategia);
                estrategias.AddRange(estrategiasEncontradas);
            }
            catch(Exception)
            {
                errorCount++;
            }
        }

        if(errorCount > 0 && estrategias.Count == 0)
        {
            throw new InvalidOperationException(
                $"Hay {errorCount} DLL(s) que no pudieron cargarse y no se encontraron estrategias validas");
        }

        return estrategias;
    }

    public bool ValidarEstrategia(Type tipo)
    {
        return tipo.IsClass &&
               !tipo.IsAbstract &&
               typeof(IPuntuacion).IsAssignableFrom(tipo) &&
               tipo.GetConstructor(Type.EmptyTypes) != null;
    }
}
