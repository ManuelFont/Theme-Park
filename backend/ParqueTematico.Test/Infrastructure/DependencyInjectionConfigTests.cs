using Infrastructure;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace Test.Infrastructure;

[TestClass]
public class DependencyInjectionConfigTests
{
    private ServiceProvider BuildProviderWithDb()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ParqueDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));
        services.AddInfrastructureServices();

        return services.BuildServiceProvider();
    }

    [TestMethod]
    public void AddInfrastructureServices_RegistraServiciosCorrectamente()
    {
        var provider = BuildProviderWithDb();

        var repo = provider.GetService<IUsuarioRepository>();

        Assert.IsNotNull(repo);
    }

    [TestMethod]
    public void AddApplicationServices_RegistraServiciosCorrectamente()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ParqueDbContext>(options =>
            options.UseInMemoryDatabase("TestDb2"));

        services.AddInfrastructureServices();
        services.AddApplicationServices();

        var provider = services.BuildServiceProvider();

        var svc = provider.GetService<IUsuarioService>();

        Assert.IsNotNull(svc);
    }
}
