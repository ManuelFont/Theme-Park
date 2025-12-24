using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using ParqueTematico.Application.Services;
using ParqueTematico.WebApi.Filtros;
using RouteData = Microsoft.AspNetCore.Routing.RouteData;

namespace Test.WebApi.Filtros;

[TestClass]
public class AuthorizationFilterAttributeTests
{
    private AuthService _authService = null!;
    private IConfiguration _configuration = null!;
    private ParqueDbContext _context = null!;
    private UsuarioDbRepository _usuarioRepo = null!;
    private UsuarioService _usuarioService = null!;

    [TestInitialize]
    public void SetUp()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "JwtSettings:SecretKey", "abcdefghijklmnopqrstuvwxyz1234567890ABCDEF" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _context = SqlContextFactory.CreateMemoryContext();
        _usuarioRepo = new UsuarioDbRepository(_context);
        _usuarioService = new UsuarioService(_usuarioRepo);
        _usuarioService.InicializarAdministrador();
        _authService = new AuthService(_configuration, _usuarioService);
    }

    [TestMethod]
    public void OnAuthorization_TokenValido_NoDebeRetornarUnauthorized()
    {
        var token = _authService.GenerarToken("admin@admin.com");
        var filtro = new AuthorizationFilterAttribute(_authService);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = $"Bearer {token}";

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        var context = new AuthorizationFilterContext(actionContext, []);

        filtro.OnAuthorization(context);

        Assert.AreEqual("admin@admin.com", context.HttpContext.User.Identity?.Name);
    }

    [TestMethod]
    public void OnAuthorization_TokenInValido_DebeRetornarUnauthorized()
    {
        var token = "token_invalido";

        var filtro = new AuthorizationFilterAttribute(_authService);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = $"Bearer {token}";

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        var context = new AuthorizationFilterContext(actionContext, []);

        filtro.OnAuthorization(context);

        Assert.IsInstanceOfType(context.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void OnAuthorization_SinHeader_DebeRetornarUnauthorized()
    {
        var filtro = new AuthorizationFilterAttribute(_authService);

        var httpContext = new DefaultHttpContext();

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        var context = new AuthorizationFilterContext(actionContext, []);

        filtro.OnAuthorization(context);

        Assert.IsInstanceOfType(context.Result, typeof(UnauthorizedResult));
    }
}
