using Infrastructure.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Infrastructure;

[TestClass]
public class CorsConfigTests
{
    [TestMethod]
    public void AddCorsPolicy_NoLanzaExcepciones()
    {
        var services = new ServiceCollection();
        services.AddCorsPolicy();
        Assert.IsTrue(services.Any());
    }
}
