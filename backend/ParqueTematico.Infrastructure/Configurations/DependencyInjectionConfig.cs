using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using ParqueTematico.Application.Plugins;
using ParqueTematico.Application.Services;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace Infrastructure.Configurations;

public static class DependencyInjectionConfig
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IAtraccionRepository, AtraccionRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioDbRepository>();
        services.AddScoped<IIncidenciaRepository, IncidenciaDbRepository>();
        services.AddScoped<IMantenimientoPreventivoRepository, MantenimientoPreventivoDbRepository>();
        services.AddScoped<IEventoRepository, EventoDbRepository>();
        services.AddScoped<IRelojRepository, RelojDbRepository>();
        services.AddScoped<ITicketRepository, TicketDbRepository>();
        services.AddScoped<IAccesoAtraccionRepository, AccesoAtraccionDbRepository>();
        services.AddScoped<IHistorialPuntuacionRepository, HistorialPuntuacionDbRepository>();
        services.AddScoped<ICanjeRepository, CanjeDbRepository>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IAtraccionService, AtraccionService>();
        services.AddScoped<IIncidenciaService, IncidenciaService>();
        services.AddScoped<IMantenimientoPreventivoService, MantenimientoPreventivoService>();
        services.AddScoped<IEventoService, EventoService>();
        services.AddScoped<IRelojService, RelojService>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IHistorialPuntuacionService, HistorialPuntuacionService>();
        services.AddScoped<IRankingService, RankingService>();
        services.AddScoped<AccesoAtraccionService>();
        services.AddScoped<IAccesoAtraccionService>(sp => sp.GetRequiredService<AccesoAtraccionService>());
        services.AddScoped<IComandoAccesoAtraccion>(sp => sp.GetRequiredService<AccesoAtraccionService>());
        services.AddScoped<IConsultaAccesoAtraccion>(sp => sp.GetRequiredService<AccesoAtraccionService>());
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICanjeService, CanjeService>();
        services.AddScoped<IRecompensaService, RecompensaService>();
        return services;
    }

    public static void InicializarAplicacion(this IServiceProvider serviceProvider, string pluginsPath)
    {
        using var scope = serviceProvider.CreateScope();

        var usuarioService = scope.ServiceProvider.GetRequiredService<IUsuarioService>();
        usuarioService.InicializarAdministrador();

        serviceProvider.InicializarPluginSystem(pluginsPath);
    }
}
