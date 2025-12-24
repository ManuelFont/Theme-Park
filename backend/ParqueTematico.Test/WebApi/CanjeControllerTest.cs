using System.Security.Claims;
using Dtos;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using ParqueTematico.Application.Services;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Controllers;

namespace Test.WebApi;

[TestClass]
public class CanjeControllerTest
{
    private AuthService _authService = null!;
    private CanjeController _controller = null!;
    private ParqueDbContext _context = null!;
    private Mock<ICanjeService> _service = null!;

    [TestInitialize]
    public void SetUp()
    {
        _service = new Mock<ICanjeService>();
        _context = SqlContextFactory.CreateMemoryContext();
        var usuarioRepo = new UsuarioDbRepository(_context);

        var inMemorySettings = new Dictionary<string, string>
        {
            { "JwtSettings:SecretKey", "abcdefghijklmnopqrstuvwxyz1234567890ABCDEF" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
        var usuarioService = new UsuarioService(usuarioRepo);
        _authService = new AuthService(configuration, usuarioService);

        _controller = new CanjeController(_service.Object, _authService);
    }

    [TestMethod]
    public void ObtenerMisCanjes_RetornaCorrectamenteCanjesDelVisitante()
    {
        var visitanteId = Guid.NewGuid();
        var lista = new List<CanjeCreadoDto>();
        var dto = new CanjeCreadoDto { Id = Guid.NewGuid(), UsuarioId = visitanteId, RecompensaId = Guid.NewGuid(), FechaCanje = DateTime.Today };
        lista.Add(dto);
        _service.Setup(s => s.ObtenerPorVisitante(visitanteId)).Returns(lista);

        var claims = new List<Claim> { new("id", visitanteId.ToString()), new("tipoUsuario", "Visitante") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        ActionResult<IList<CanjeCreadoDto>> result = _controller.ObtenerMisCanjes();

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(lista, okResult.Value);
    }

    [TestMethod]
    public void Obtener_RetornaRecurso()
    {
        var dto = new CanjeCreadoDto
        {
            Id = Guid.NewGuid(),
            UsuarioId = Guid.NewGuid(),
            RecompensaId = Guid.NewGuid(),
            FechaCanje = DateTime.UtcNow
        };
        _service.Setup(s => s.ObtenerCanje(dto.Id)).Returns(dto);

        ActionResult<CanjeCreadoDto> result = _controller.Obtener(dto.Id);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(dto, okResult.Value);
    }

    [TestMethod]
    public void Crear_DeberiaRetornarCreatedConUbicacion()
    {
        var visitanteId = Guid.NewGuid();
        var request = new CanjearRecompensaRequest
        {
            RecompensaId = Guid.NewGuid()
        };
        var dtoCreado = new CanjeCreadoDto
        {
            Id = Guid.NewGuid(),
            UsuarioId = visitanteId,
            RecompensaId = request.RecompensaId,
            FechaCanje = DateTime.UtcNow
        };
        _service.Setup(s => s.CrearCanje(It.IsAny<CanjeCrearDto>())).Returns(dtoCreado);

        var claims = new List<Claim> { new("id", visitanteId.ToString()), new("tipoUsuario", "Visitante") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        ActionResult<CanjeCreadoDto> result = _controller.Crear(request);

        var createdResult = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(201, createdResult.StatusCode);
        Assert.AreEqual(dtoCreado, createdResult.Value);
        Assert.AreEqual(nameof(_controller.Obtener), createdResult.ActionName);
    }

    [TestMethod]
    public void Actualizar_DeberiaRetornarOkConCanjeActualizado()
    {
        var id = Guid.NewGuid();
        var dtoActualizar = new CanjeCrearDto
        {
            UsuarioId = Guid.NewGuid(),
            RecompensaId = Guid.NewGuid()
        };
        var dtoActualizado = new CanjeCreadoDto
        {
            Id = id,
            UsuarioId = dtoActualizar.UsuarioId,
            RecompensaId = dtoActualizar.RecompensaId,
            FechaCanje = DateTime.UtcNow
        };
        _service.Setup(s => s.ActualizarCanje(id, dtoActualizar)).Returns(dtoActualizado);

        ActionResult<CanjeCreadoDto> result = _controller.Actualizar(id, dtoActualizar);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        var returnedDto = okResult.Value as CanjeCreadoDto;
        Assert.IsNotNull(returnedDto);
        Assert.AreEqual(dtoActualizado.Id, returnedDto.Id);
    }

    [TestMethod]
    public void Eliminar_DeberiaRetornarNoContent()
    {
        var id = Guid.NewGuid();
        _service.Setup(s => s.EliminarCanje(id));

        IActionResult result = _controller.Eliminar(id);

        var noContent = result as NoContentResult;
        Assert.IsNotNull(noContent);
        Assert.AreEqual(204, noContent.StatusCode);
        _service.Verify(s => s.EliminarCanje(id), Times.Once);
    }
}
