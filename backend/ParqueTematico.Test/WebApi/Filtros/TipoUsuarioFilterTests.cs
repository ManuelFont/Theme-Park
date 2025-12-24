using Dominio.Entities.Usuarios;
using Dtos;
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
public class TipoUsuarioFilterTests
{
    private AuthService _authService = null!;
    private IConfiguration _configuration = null!;
    private ParqueDbContext _context = null!;
    private UsuarioDbRepository _usuarioRepo = null!;
    private UsuarioService _usuarioService = null!;
    private TipoUsuarioFilter filtro = null!;

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

        filtro = new TipoUsuarioFilter(["Administrador"], _authService);
    }

    [TestMethod]
    public void OnAuthorization_TokenValido_NoDebeRetornarUnauthorized()
    {
        var token = _authService.GenerarToken("admin@admin.com");

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
    public void OnAuthorization_SinHeader_DebeRetornarUnauthorized()
    {
        var httpContext = new DefaultHttpContext();

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        var context = new AuthorizationFilterContext(actionContext, []);

        filtro.OnAuthorization(context);

        Assert.IsInstanceOfType(context.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void OnAuthorization_TokenInvalido_DebeRetornarUnauthorized()
    {
        var token = "token_invalido";

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
    public void OnAuthorization_UsuarioNoAdmin_DebeRetornarUnauthorized()
    {
        var request = new CrearUsuarioRequest
        {
            Nombre = "Manuel",
            Apellido = "Font",
            Email = "manuel@mail.com",
            Contrasenia = "12345678Aa!",
            TipoUsuario = "Visitante",
            FechaNacimiento = DateTime.Today,
            NivelMembresia = NivelMembresia.Estandar
        };
        _usuarioService.CrearUsuario(request);

        var token = _authService.GenerarToken("manuel@mail.com");

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = $"Bearer {token}";

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        var context = new AuthorizationFilterContext(actionContext, []);

        filtro.OnAuthorization(context);

        Assert.IsInstanceOfType(context.Result, typeof(ForbidResult));
    }
}
