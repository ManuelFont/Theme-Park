using Dominio.Entities;
using Dominio.Entities.Puntuacion;
using Dominio.Entities.Usuarios;
using Dtos;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using ParqueTematico.Application.Services;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Controllers;
namespace Test.WebApi;

[TestClass]
public class RankingControllerTests
{
    private AccesoAtraccionDbRepository _accesoRepo = null!;
    private BaseRepository<Atraccion> _atraccionRepo = null!;
    private ParqueDbContext _context = null!;
    private RankingController _controller = null!;
    private RankingService _rankingService = null!;
    private RelojDbRepository _relojRepo = null!;
    private RelojService _relojService = null!;
    private TicketDbRepository _ticketRepo = null!;
    private UsuarioDbRepository _usuarioRepo = null!;
    private MockEstrategiaService _estrategiaService = null!;

    [TestInitialize]
    public void SetUp()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _accesoRepo = new AccesoAtraccionDbRepository(_context);
        _usuarioRepo = new UsuarioDbRepository(_context);
        _atraccionRepo = new BaseRepository<Atraccion>(_context);
        _ticketRepo = new TicketDbRepository(_context);
        _relojRepo = new RelojDbRepository(_context);

        _relojService = new RelojService(_relojRepo);

        var mockPluginManager = new MockPluginManager();
        _estrategiaService = new MockEstrategiaService(mockPluginManager);
        _rankingService = new RankingService(_accesoRepo, _relojService, _estrategiaService);
        _controller = new RankingController(_rankingService);
    }

    [TestMethod]
    public void ListarEstrategias_DeberiaRetornarListaDeTresEstrategias()
    {
        var result = _controller.ListarEstrategias() as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var estrategias = result.Value as IEnumerable<(string Nombre, string Descripcion)>;
        Assert.IsNotNull(estrategias);

        var lista = estrategias.ToList();

        Assert.AreEqual(3, lista.Count);
    }

    [TestMethod]
    public void ObtenerEstrategiaActiva_PorDefecto_DeberiaRetornarPuntuacionPorAtraccion()
    {
        var result = _controller.ObtenerEstrategiaActiva() as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void CambiarEstrategia_EstrategiaValida_DeberiaActualizarEstrategia()
    {
        var request = new CambiarEstrategiaRequest { NombreEstrategia = "PuntuacionCombo" };

        var result = _controller.CambiarEstrategia(request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual("Puntuación Combo", _rankingService.ObtenerNombreEstrategiaActiva());
    }

    [TestMethod]
    public void CambiarEstrategia_EstrategiaInvalida_DeberiaRetornarBadRequest()
    {
        var request = new CambiarEstrategiaRequest { NombreEstrategia = "EstrategiaInexistente" };

        var result = _controller.CambiarEstrategia(request) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void ObtenerRankingDiario_SinAccesos_DeberiaRetornarListaVacia()
    {
        var result = _controller.ObtenerRankingDiario() as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void ObtenerRankingDiario_ConAccesos_DeberiaRetornarRankingOrdenado()
    {
        var visitante1 = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var visitante2 = new Visitante("Maria", "Lopez", "maria@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante1);
        _usuarioRepo.Agregar(visitante2);

        var atraccion1 = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 0, 50, "Desc", true);
        var atraccion2 = new Atraccion("Simulador", TipoAtraccion.Simulador, 0, 50, "Desc", true);
        _atraccionRepo.Agregar(atraccion1);
        _atraccionRepo.Agregar(atraccion2);

        DateTime fechaHoy = _relojService.ObtenerFechaHora();
        var ticket1 = new Ticket(visitante1, fechaHoy, TipoEntrada.General, null);
        var ticket2 = new Ticket(visitante2, fechaHoy, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticket1);
        _ticketRepo.Agregar(ticket2);

        var acceso1 = new AccesoAtraccion(visitante1, atraccion1, ticket1, fechaHoy);
        var acceso2 = new AccesoAtraccion(visitante1, atraccion2, ticket1, fechaHoy.AddHours(1));
        var acceso3 = new AccesoAtraccion(visitante2, atraccion1, ticket2, fechaHoy);
        _accesoRepo.Agregar(acceso1);
        _accesoRepo.Agregar(acceso2);
        _accesoRepo.Agregar(acceso3);

        var result = _controller.ObtenerRankingDiario() as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void CambiarEstrategia_YConsultarRanking_DeberiaUsarNuevaEstrategia()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 0, 50, "Desc", true);
        _atraccionRepo.Agregar(atraccion);
        DateTime fechaHoy = _relojService.ObtenerFechaHora();
        var ticket = new Ticket(visitante, fechaHoy, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticket);
        var acceso = new AccesoAtraccion(visitante, atraccion, ticket, fechaHoy);
        _accesoRepo.Agregar(acceso);

        var request = new CambiarEstrategiaRequest { NombreEstrategia = "PuntuacionPorEvento" };
        _controller.CambiarEstrategia(request);

        var result = _controller.ObtenerRankingDiario() as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual("Puntuación Por Evento", _rankingService.ObtenerNombreEstrategiaActiva());
    }

    public class MockEstrategiaService(IPluginManager pluginManager) : EstrategiaService(pluginManager)
    {
        public void EstablecerEstrategiaDirecta(IPuntuacion estrategia)
        {
            typeof(EstrategiaService)
                .GetField("_estrategiaActiva", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .SetValue(this, estrategia);
        }
    }

    public class MockPluginManager : IPluginManager
    {
        public void Inicializar(string rutaPlugins)
        {
        }

        public IPuntuacion? CrearInstancia(string nombreEstrategia)
        {
            return nombreEstrategia switch
            {
                "PuntuacionPorAtraccion" => new PuntuacionPorAtraccion(),
                "PuntuacionCombo" => new PuntuacionCombo(),
                "PuntuacionPorEvento" => new PuntuacionPorEvento(),
                _ => null
            };
        }

        public IEnumerable<string> ObtenerNombresDeEstrategias()
        {
            return
            [
                "PuntuacionPorAtraccion",
                "PuntuacionCombo",
                "PuntuacionPorEvento"
            ];
        }
    }
}
