using Dominio.Entities.Usuarios;
using Dtos;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using ParqueTematico.Application.Services;
using ParqueTematico.WebApi.Controllers;

namespace Test.WebApi;

[TestClass]
public class AuthControllerTests
{
    private ParqueDbContext _context = null!;
    private UsuarioDbRepository _repoUsuarios = null!;
    private CrearUsuarioRequest _request = null!;
    private AuthService _service = null!;
    private UsuarioService _usuarioService = null!;

    [TestInitialize]
    public void SetUp()
    {
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["JwtSettings:SecretKey"])
            .Returns("abcdefghijklmnopqrstuvwxyz1234567890ABCDEF");

        _context = SqlContextFactory.CreateMemoryContext();
        _repoUsuarios = new UsuarioDbRepository(_context);
        _usuarioService = new UsuarioService(_repoUsuarios);
        _service = new AuthService(mockConfig.Object, _usuarioService);

        _request = new CrearUsuarioRequest
        {
            Nombre = "Nahuel",
            Apellido = "Mileo",
            Email = "nahuel@mail.com",
            Contrasenia = "Pass123!",
            TipoUsuario = "visitante",
            FechaNacimiento = new DateTime(2000, 1, 1),
            NivelMembresia = NivelMembresia.Premium
        };
        _usuarioService.CrearUsuario(_request);
    }

    [TestMethod]
    public void AuthController_ParamsCorrectos_ControllerCreado()
    {
        var controller = new AuthController(_service);
        Assert.IsNotNull(controller);
    }

    [TestMethod]
    public void Login_RequestCorrecto_RetornaToken()
    {
        var responseEsperado = new LoginResponse(_service.GenerarToken("nahuel@mail.com"), DateTime.Now);
        var controller = new AuthController(_service);
        var loginRequest = new LoginRequest("nahuel@mail.com", "Pass123!");
        var okResult = controller.Login(loginRequest) as OkObjectResult;
        Assert.IsNotNull(okResult);
        var loginResponse = okResult.Value as LoginResponse;
        Assert.IsNotNull(loginResponse);
        Assert.IsNotNull(responseEsperado.Token);
        Assert.IsTrue(loginResponse.ExpiraEn > responseEsperado.ExpiraEn);
    }

    [TestMethod]
    public void Login_RequestInvalido_RetornaUnauthorized()
    {
        var controller = new AuthController(_service);
        var loginRequest = new LoginRequest("Nahuel", "12345678!Aa");
        IActionResult result = controller.Login(loginRequest);

        var unauthorized = result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorized, "El resultado no fue Unauthorized");

        var value = unauthorized.Value;
        Assert.IsNotNull(value);

        var mensajeProp = value.GetType().GetProperty("mensaje");
        Assert.IsNotNull(mensajeProp, "La propiedad 'mensaje' no existe");

        var mensaje = mensajeProp.GetValue(value)?.ToString();
        Assert.AreEqual("Credenciales inválidas", mensaje);
    }
}
