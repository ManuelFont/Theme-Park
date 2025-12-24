using System.Security.Claims;
using Dominio.Entities.Usuarios;
using Dominio.Exceptions;
using Dtos;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ParqueTematico.Application.Services;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Controllers;

namespace Test.WebApi;

[TestClass]
public class UsuarioControllerTests
{
    private ParqueDbContext _context = null!;
    private UsuarioDbRepository _repo = null!;
    private UsuarioService _service = null!;
    private CrearUsuarioRequest _request = null!;
    private Mock<IAuthService> _mockAuthService = null!;

    [TestInitialize]
    public void SetUp()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _repo = new UsuarioDbRepository(_context);
        _service = new UsuarioService(_repo);
        _mockAuthService = new Mock<IAuthService>();
        _mockAuthService.Setup(a => a.ObtenerUserIdDeClaims(It.IsAny<ClaimsPrincipal>()))
            .Returns<ClaimsPrincipal>(user =>
            {
                var idClaim = user.FindFirst("id");
                if(idClaim == null)
                {
                    throw new InvalidOperationException("No se encontró el claim 'id'");
                }

                return Guid.Parse(idClaim.Value);
            });

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
    }

    [TestMethod]
    public void UsuarioController_ParamsCorrectos_ControladorCreado()
    {
        var controller = new UsuarioController(_service, _mockAuthService.Object);
        Assert.IsNotNull(controller);
    }

    [TestMethod]
    public void CrearUsuario_RequestCorrecto_UsuarioGuardado()
    {
        var controller = new UsuarioController(_service, _mockAuthService.Object);

        var result = controller.CrearUsuario(_request);
        var createdResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult, "Se esperaba CreatedAtActionResult");

        var usuarioResponse = createdResult.Value as UsuarioDTO;
        Assert.IsNotNull(usuarioResponse);

        var usuarioGuardado = _repo.ObtenerTodos().FirstOrDefault();
        Assert.IsNotNull(usuarioGuardado);

        Assert.AreEqual(usuarioGuardado.Id, usuarioResponse.Id);
        Assert.AreEqual(usuarioGuardado.Nombre, usuarioResponse.Nombre);
        Assert.AreEqual(usuarioGuardado.Apellido, usuarioResponse.Apellido);
        Assert.AreEqual(usuarioGuardado.Email, usuarioResponse.Email);
    }

    [TestMethod]
    public void DevolverListaVacia_DebeDevolverVacio()
    {
        var controller = new UsuarioController(_service, _mockAuthService.Object);

        var result = controller.ListarUsuarios();
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var lista = okResult.Value as List<UsuarioDTO>;
        Assert.IsNotNull(lista);
        Assert.AreEqual(0, lista.Count);
    }

    [TestMethod]
    public void ListarUsuarios_ConUsuarios_DebeDevolverUsuarios()
    {
        var controller = new UsuarioController(_service, _mockAuthService.Object);
        controller.CrearUsuario(_request);

        var result = controller.ListarUsuarios();
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var lista = okResult.Value as List<UsuarioDTO>;
        Assert.IsNotNull(lista);
        Assert.AreEqual(1, lista.Count);
        Assert.AreEqual("Nahuel", lista[0].Nombre);
        Assert.AreEqual("Mileo", lista[0].Apellido);
    }

    [TestMethod]
    public void ObtenerUsuarioPorId_DevuelveUsuario()
    {
        var controller = new UsuarioController(_service, _mockAuthService.Object);

        var create = controller.CrearUsuario(_request) as CreatedAtActionResult;
        var usuarioResponse = create!.Value as UsuarioDTO;

        var usuario = controller.ObtenerUsuarioPorId(usuarioResponse!.Id);

        var okResult = usuario as OkObjectResult;
        Assert.IsNotNull(okResult);

        var userResp = okResult.Value as UsuarioDTO;
        Assert.IsNotNull(userResp);
        Assert.AreEqual(usuarioResponse.Id, userResp.Id);
        Assert.AreEqual("Nahuel", userResp.Nombre);
        Assert.AreEqual("Mileo", userResp.Apellido);
        Assert.AreEqual("nahuel@mail.com", userResp.Email);
    }

    [TestMethod]
    public void ObtenerUsuarioPorId_DevuelveNotFound()
    {
        var controller = new UsuarioController(_service, _mockAuthService.Object);
        var usuario = controller.ObtenerUsuarioPorId(Guid.NewGuid());

        Assert.IsInstanceOfType(usuario, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public void Actualizar_UsuarioExistente_RetornaNoContent()
    {
        var controller = new UsuarioController(_service, _mockAuthService.Object);

        var create = controller.CrearUsuario(_request) as CreatedAtActionResult;
        var usuario = create!.Value as UsuarioDTO;

        var req = new ActualizarUsuarioRequest
        {
            Nombre = "Nuevo",
            Apellido = "Apellido",
            Email = "nuevo@mail.com",
            Contrasenia = "Password123!",
            FechaNacimiento = new DateTime(2000, 1, 1),
            NivelMembresia = NivelMembresia.Premium
        };

        var result = controller.Actualizar(usuario!.Id, req);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        var actualizado = _repo.ObtenerPorId(usuario.Id);
        Assert.AreEqual("Nuevo", actualizado!.Nombre);
    }

    [TestMethod]
    public void Actualizar_UsuarioInvalido_LanzaExcepcion()
    {
        var controller = new UsuarioController(_service, _mockAuthService.Object);
        var create = controller.CrearUsuario(_request) as CreatedAtActionResult;
        var usuario = create!.Value as UsuarioDTO;

        var req = new ActualizarUsuarioRequest
        {
            Nombre = string.Empty,
            Apellido = "X",
            Email = "mail@mail.com"
        };

        Assert.ThrowsException<UserException>(() => controller.Actualizar(usuario!.Id, req));
    }

    [TestMethod]
    public void Actualizar_UsuarioNoExiste_LanzaExcepcion()
    {
        var controller = new UsuarioController(_service, _mockAuthService.Object);

        var req = new ActualizarUsuarioRequest
        {
            Nombre = "X",
            Apellido = "Y",
            Email = "x@y.com"
        };

        Assert.ThrowsException<KeyNotFoundException>(() => controller.Actualizar(Guid.NewGuid(), req));
    }

    [TestMethod]
    public void Eliminar_UsuarioExistente_DebeEliminarUsuario()
    {
        var controller = new UsuarioController(_service, _mockAuthService.Object);
        var create = controller.CrearUsuario(_request) as CreatedAtActionResult;
        var usuario = create!.Value as UsuarioDTO;

        var result = controller.Eliminar(usuario!.Id);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        Assert.IsNull(_repo.ObtenerPorId(usuario.Id));
    }

    [TestMethod]
    public void Eliminar_UsuarioNoExiste_LanzaExcepcion()
    {
        var controller = new UsuarioController(_service, _mockAuthService.Object);
        Assert.ThrowsException<NotFoundException>(() => controller.Eliminar(Guid.NewGuid()));
    }

    [TestMethod]
    public void ObtenerMiInformacion_UsuarioExiste_DebeRetornarUsuario()
    {
        var controller = new UsuarioController(_service, _mockAuthService.Object);
        var create = controller.CrearUsuario(_request) as CreatedAtActionResult;
        var usuario = create!.Value as UsuarioDTO;

        var claims = new List<Claim> { new("id", usuario!.Id.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var result = controller.ObtenerMiInformacion();
        var okResult = result as OkObjectResult;

        Assert.IsNotNull(okResult);
        var userResp = okResult.Value as UsuarioDTO;
        Assert.IsNotNull(userResp);
        Assert.AreEqual(usuario.Id, userResp.Id);
        Assert.AreEqual("Nahuel", userResp.Nombre);
        Assert.AreEqual("Mileo", userResp.Apellido);
        Assert.AreEqual("nahuel@mail.com", userResp.Email);
    }

    [TestMethod]
    public void ObtenerMiInformacion_UsuarioNoExiste_DebeRetornarNotFound()
    {
        var controller = new UsuarioController(_service, _mockAuthService.Object);

        var claims = new List<Claim> { new("id", Guid.NewGuid().ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var result = controller.ObtenerMiInformacion();

        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }
}
