using System.Security.Claims;
using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Dtos;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ParqueTematico.Application.Services;
using ParqueTematico.WebApi.Controllers;
namespace Test.WebApi;

[TestClass]
public class TicketControllerTests
{
    private AuthService _authService = null!;
    private ParqueDbContext _context = null!;
    private EventoDbRepository _eventoRepo = null!;
    private RelojDbRepository _relojRepo = null!;
    private TicketService _service = null!;
    private TicketDbRepository _ticketRepo = null!;
    private UsuarioDbRepository _usuarioRepo = null!;

    [TestInitialize]
    public void SetUp()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _ticketRepo = new TicketDbRepository(_context);
        _usuarioRepo = new UsuarioDbRepository(_context);
        _eventoRepo = new EventoDbRepository(_context);
        _relojRepo = new RelojDbRepository(_context);
        var atraccionRepo = new AtraccionRepository(_context);
        var atraccionService = new AtraccionService(atraccionRepo);
        var eventoService = new EventoService(_eventoRepo, atraccionService);
        var relojService = new RelojService(_relojRepo);
        _service = new TicketService(_ticketRepo, _usuarioRepo, eventoService, relojService);

        var inMemorySettings = new Dictionary<string, string>
        {
            { "JwtSettings:SecretKey", "abcdefghijklmnopqrstuvwxyz1234567890ABCDEF" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
        var usuarioService = new UsuarioService(_usuarioRepo);
        _authService = new AuthService(configuration, usuarioService);
    }

    [TestMethod]
    public void TicketController_ParamsCorrectos_ControladorCreado()
    {
        var controller = new TicketController(_service, _authService);
        Assert.IsNotNull(controller);
    }

    [TestMethod]
    public void ComprarTicket_ConRequestValido_DeberiaCrearTicket()
    {
        var controller = new TicketController(_service, _authService);
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);

        var claims = new List<Claim>
        {
            new Claim("id", visitante.Id.ToString()),
            new Claim("tipoUsuario", "Visitante")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var request = new ComprarTicketRequest
        {
            FechaVisita = DateTime.Now.AddDays(10),
            TipoEntrada = TipoEntrada.General,
            EventoId = null
        };

        IActionResult result = controller.ComprarTicket(request);

        var createdResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);

        IList<Ticket> tickets = _ticketRepo.ObtenerTodos();
        Assert.AreEqual(1, tickets.Count);
    }

    [TestMethod]
    public void ObtenerPorId_TicketExiste_DeberiaRetornarTicket()
    {
        var controller = new TicketController(_service, _authService);
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        Guid ticketId = _service.ComprarTicket(visitante.Id, DateTime.Now.AddDays(10), TipoEntrada.General, null);

        ActionResult<TicketDto> result = controller.ObtenerPorId(ticketId);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var ticketDto = okResult.Value as TicketDto;
        Assert.IsNotNull(ticketDto);
        Assert.AreEqual(ticketId, ticketDto.Id);
    }

    [TestMethod]
    public void ObtenerPorId_TicketNoExiste_DeberiaRetornarNotFound()
    {
        var controller = new TicketController(_service, _authService);

        ActionResult<TicketDto> result = controller.ObtenerPorId(Guid.NewGuid());

        var notFoundResult = result.Result as NotFoundResult;
        Assert.IsNotNull(notFoundResult);
    }

    [TestMethod]
    public void ObtenerPorVisitante_VisitanteConTickets_DeberiaRetornarLista()
    {
        var controller = new TicketController(_service, _authService);
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        _service.ComprarTicket(visitante.Id, DateTime.Now.AddDays(10), TipoEntrada.General, null);
        _service.ComprarTicket(visitante.Id, DateTime.Now.AddDays(20), TipoEntrada.General, null);

        ActionResult<List<TicketDto>> result = controller.ObtenerPorVisitante(visitante.Id);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var lista = okResult.Value as List<TicketDto>;
        Assert.IsNotNull(lista);
        Assert.AreEqual(2, lista.Count);
    }

    [TestMethod]
    public void ObtenerPorVisitante_VisitanteSinTickets_DeberiaRetornarListaVacia()
    {
        var controller = new TicketController(_service, _authService);
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);

        ActionResult<List<TicketDto>> result = controller.ObtenerPorVisitante(visitante.Id);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var lista = okResult.Value as List<TicketDto>;
        Assert.IsNotNull(lista);
        Assert.AreEqual(0, lista.Count);
    }
}
