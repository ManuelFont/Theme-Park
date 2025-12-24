using Microsoft.Extensions.DependencyInjection;
using ParqueTematico.Application.Services;
using ParqueTematico.BusinessLogicInterface;

namespace ParqueTematico.Application.Plugins;

public static class PluginServiceExtensions
{
    public static IServiceCollection AddPluginSystem(this IServiceCollection services)
    {
        services.AddSingleton<IPluginLoader, PluginLoader>();
        services.AddSingleton<IPluginManager, PluginManager>();
        services.AddSingleton<IEstrategiaService, EstrategiaService>();

        return services;
    }

    public static void InicializarPluginSystem(this IServiceProvider serviceProvider, string pluginsPath)
    {
        var pluginManager = serviceProvider.GetRequiredService<IPluginManager>();
        pluginManager.Inicializar(pluginsPath);
    }
}
