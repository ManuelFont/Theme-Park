using System.IdentityModel.Tokens.Jwt;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using ParqueTematico.Application.Services;

namespace Test.Application;

[TestClass]
public class AuthServiceTests
{
    private IConfiguration _configuration = null!;
    private ParqueDbContext _context = null!;
    private UsuarioDbRepository _usuarioRepo = null!;
    private UsuarioService _usuarioService = null!;

    [TestInitialize]
    public void Setup()
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
    }

    [TestMethod]
    public void GenerarToken_UsuarioValido_DevuelveToken()
    {
        var authService = new AuthService(_configuration, _usuarioService);

        var token = authService.GenerarToken("admin@admin.com");

        Assert.IsFalse(string.IsNullOrEmpty(token), "El token no debería ser vacío");
        Assert.IsTrue(token.Split('.').Length == 3, "El token no es valido (debe tener 3 partes)");
    }

    [TestMethod]
    [ExpectedException(typeof(NullReferenceException))]
    public void GenerarToken_SinClave_ThrowsExcepcion()
    {
        IConfigurationRoot emptyConfig =
            new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>()!).Build();
        var authService = new AuthService(emptyConfig, _usuarioService);

        authService.GenerarToken("usuario");
    }

    [TestMethod]
    public void GenerarToken_UsuarioValido_IncluyeClaimId()
    {
        var authService = new AuthService(_configuration, _usuarioService);

        var token = authService.GenerarToken("admin@admin.com");

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var claimId = jwtToken.Claims.FirstOrDefault(c => c.Type == "id");
        Assert.IsNotNull(claimId, "El token debe incluir el claim 'id'");
        Assert.IsFalse(string.IsNullOrEmpty(claimId.Value), "El claim 'id' no debe estar vacío");
    }

    [TestMethod]
    public void GenerarToken_UsuarioValido_IncluyeClaimEmail()
    {
        var authService = new AuthService(_configuration, _usuarioService);

        var token = authService.GenerarToken("admin@admin.com");

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var claimEmail = jwtToken.Claims.FirstOrDefault(c => c.Type == "email");
        Assert.IsNotNull(claimEmail);
        Assert.AreEqual("admin@admin.com", claimEmail.Value);
    }

    [TestMethod]
    public void GenerarToken_UsuarioValido_IncluyeClaimTipoUsuario()
    {
        var authService = new AuthService(_configuration, _usuarioService);

        var token = authService.GenerarToken("admin@admin.com");

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var claimTipo = jwtToken.Claims.FirstOrDefault(c => c.Type == "tipoUsuario");
        Assert.IsNotNull(claimTipo);
        Assert.AreEqual("Administrador", claimTipo.Value);
    }
}
