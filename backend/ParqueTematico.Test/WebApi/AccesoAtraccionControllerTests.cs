using System.Security.Claims;
using Dominio.Entities;
using Dominio.Entities.Puntuacion;
using Dominio.Entities.Usuarios;
using Dominio.Exceptions;
using Dtos;
using Infrastructure;
using Infrastructure.Configurations;
using Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ParqueTematico.Application.Plugins;
using ParqueTematico.Application.Services;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Controllers;
using RepositoryInterfaces;

namespace Test.WebApi;

[TestClass]
public class AccesoAtraccionControllerTests
{
    private IServiceProvider _serviceProvider = null!;
    private ParqueDbContext _context = null!;
    private AccesoAtraccionController _controller = null!;
    private IComandoAccesoAtraccion _comandoService = null!;
    private IConsultaAccesoAtraccion _consultaService = null!;
    private AuthService _authService = null!;

    private IBaseRepository<Atraccion> _atraccionRepo = null!;
    private ITicketRepository _ticketRepo = null!;
    private IUsuarioRepository _usuarioRepo = null!;

    private Atraccion _atraccion = null!;
    private Ticket _ticket = null!;
    private Visitante _visitante = null!;

    [TestInitialize]
    public void SetUp()
    {
        _context = SqlContextFactory.CreateMemoryContext();

        var services = new ServiceCollection();

        services.AddSingleton(_context);

        services.AddInfrastructureServices();
        services.AddApplicationServices();
        services.AddPluginSystem();

        var inMemorySettings = new Dictionary<string, string>
        {
            { "JwtSettings:SecretKey", "abcdefghijklmnopqrstuvwxyz1234567890ABCDEF" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        services.AddScoped<AuthService>();

        _serviceProvider = services.BuildServiceProvider();

        _comandoService = _serviceProvider.GetRequiredService<IComandoAccesoAtraccion>();
        _consultaService = _serviceProvider.GetRequiredService<IConsultaAccesoAtraccion>();
        _authService = _serviceProvider.GetRequiredService<AuthService>();

        _atraccionRepo = _serviceProvider.GetRequiredService<IBaseRepository<Atraccion>>();
        _ticketRepo = _serviceProvider.GetRequiredService<ITicketRepository>();
        _usuarioRepo = _serviceProvider.GetRequiredService<IUsuarioRepository>();

        RegistrarVisitanteAtraccionTicket();
    }

    private void RegistrarVisitanteAtraccionTicket()
    {
        _controller = new AccesoAtraccionController(_comandoService, _consultaService, _authService);
        _visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(_visitante);
        _atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        _atraccionRepo.Agregar(_atraccion);
        _ticket = new Ticket(_visitante, DateTime.Now, TipoEntrada.General, null);
        _ticketRepo.Agregar(_ticket);
    }

    [TestMethod]
    public void AccesoAtraccionController_ParamsCorrectos_ControladorCreado()
    {
        var controller = new AccesoAtraccionController(_comandoService, _consultaService, _authService);
        Assert.IsNotNull(controller);
    }

    [TestMethod]
    public void RegistrarIngreso_ConRequestValido_DeberiaRetornarOk()
    {
        var request = new RegistrarIngresoRequest { TicketId = _ticket.Id, AtraccionId = _atraccion.Id };

        IActionResult result = _controller.RegistrarIngreso(request);

        var createdResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(201, createdResult.StatusCode);
    }

    [TestMethod]
    public void ObtenerAforo_ConAtraccionValida_DeberiaRetornarAforo()
    {
        _comandoService.RegistrarIngreso(_ticket.Id, _atraccion.Id);

        IActionResult result = _controller.ObtenerAforo(_atraccion.Id);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [TestMethod]
    public void ObtenerPorId_ConIdValido_DeberiaRetornarDto()
    {
        Guid accesoId = _comandoService.RegistrarIngreso(_ticket.Id, _atraccion.Id);

        ActionResult<AccesoAtraccionDto> result = _controller.ObtenerPorId(accesoId);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var dto = okResult.Value as AccesoAtraccionDto;
        Assert.IsNotNull(dto);
        Assert.AreEqual(accesoId, dto.Id);
    }

    [TestMethod]
    public void ObtenerPorId_ConIdInvalido_DeberiaRetornarNotFound()
    {
        var controller = new AccesoAtraccionController(_comandoService, _consultaService, _authService);

        ActionResult<AccesoAtraccionDto> result = controller.ObtenerPorId(Guid.NewGuid());

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void RegistrarEgreso_ConAccesoValido_DeberiaRetornarOk()
    {
        Guid accesoId = _comandoService.RegistrarIngreso(_ticket.Id, _atraccion.Id);

        IActionResult result = _controller.RegistrarEgreso(accesoId);

        var noContentResult = result as NoContentResult;
        Assert.IsNotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);
    }

    [TestMethod]
    [ExpectedException(typeof(AccesoAtraccionException))]
    public void RegistrarEgreso_ConAccesoInvalido_DeberiaLanzarExcepcion()
    {
        _controller.RegistrarEgreso(Guid.NewGuid());
    }

    [TestMethod]
    public void ObtenerReporteUsoAtracciones_DeberiaRetornarOkConLista()
    {
        Guid accesoId = _comandoService.RegistrarIngreso(_ticket.Id, _atraccion.Id);
        _controller.RegistrarEgreso(accesoId);
        DateTime fechaInicio = DateTime.Today.AddDays(-1);
        DateTime fechaFin = DateTime.Today.AddDays(1);

        ActionResult<List<ReporteUsoAtraccionDto>> result =
            _controller.ObtenerReporteUsoAtracciones(fechaInicio, fechaFin);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        var reporte = okResult.Value as List<ReporteUsoAtraccionDto>;
        Assert.IsNotNull(reporte);
    }

    [TestMethod]
    public void ObtenerAtraccionesVisitadas_DeberiaRetornarOkConAccesos()
    {
        _comandoService.RegistrarIngreso(_ticket.Id, _atraccion.Id);
        DateTime fecha = DateTime.Today;

        ActionResult<List<AccesoAtraccionDto>> result = _controller.ObtenerAtraccionesVisitadas(_visitante.Id, fecha);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        var dtos = okResult.Value as List<AccesoAtraccionDto>;
        Assert.IsNotNull(dtos);
        Assert.IsTrue(dtos.Count >= 0);
    }

    [TestMethod]
    public void ObtenerAtraccionesVisitadas_SinAccesos_DeberiaRetornarListaVacia()
    {
        DateTime fecha = DateTime.Today;

        ActionResult<List<AccesoAtraccionDto>> result = _controller.ObtenerAtraccionesVisitadas(_visitante.Id, fecha);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var dtos = okResult.Value as List<AccesoAtraccionDto>;
        Assert.IsNotNull(dtos);
        Assert.AreEqual(0, dtos.Count);
    }

    [TestMethod]
    public void ObtenerMiHistorial_ConAccesos_DeberiaRetornarOkConLista()
    {
        _comandoService.RegistrarIngreso(_ticket.Id, _atraccion.Id);
        DateTime fecha = DateTime.Today;

        var claims = new List<Claim>
        {
            new Claim("id", _visitante.Id.ToString()),
            new Claim("tipoUsuario", "Visitante")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        ActionResult<List<AccesoAtraccionDto>> result = _controller.ObtenerMiHistorial(fecha);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        var dtos = okResult.Value as List<AccesoAtraccionDto>;
        Assert.IsNotNull(dtos);
        Assert.IsTrue(dtos.Count > 0);
    }

    [TestMethod]
    public void ObtenerMiHistorial_SinAccesos_DeberiaRetornarListaVacia()
    {
        DateTime fecha = DateTime.Today;

        var claims = new List<Claim>
        {
            new Claim("id", _visitante.Id.ToString()),
            new Claim("tipoUsuario", "Visitante")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        ActionResult<List<AccesoAtraccionDto>> result = _controller.ObtenerMiHistorial(fecha);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var dtos = okResult.Value as List<AccesoAtraccionDto>;
        Assert.IsNotNull(dtos);
        Assert.AreEqual(0, dtos.Count);
    }

    public class MockPluginManager : IPluginManager
    {
        public void Inicializar(string rutaPlugins)
        {
        }

        public IPuntuacion? CrearInstancia(string nombreEstrategia)
        {
            return null;
        }

        public IEnumerable<string> ObtenerNombresDeEstrategias()
        {
            return [];
        }
    }
}
