using Dominio.Entities;
using Dominio.Entities.Puntuacion;
using Dominio.Entities.Usuarios;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using ParqueTematico.Application.Services;
using ParqueTematico.BusinessLogicInterface;

namespace Test.Application;

[TestClass]
public class RankingServiceTests
{
    private AccesoAtraccionDbRepository _accesoRepo = null!;
    private BaseRepository<Atraccion> _atraccionRepo = null!;
    private ParqueDbContext _context = null!;
    private RelojDbRepository _relojRepo = null!;
    private RelojService _relojService = null!;
    private RankingService _service = null!;
    private TicketDbRepository _ticketRepo = null!;
    private UsuarioDbRepository _usuarioRepo = null!;
    private MockEstrategiaService _estrategiaService = null!;

    [TestInitialize]
    public void SetUp()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _accesoRepo = new AccesoAtraccionDbRepository(_context);
        _ticketRepo = new TicketDbRepository(_context);
        _atraccionRepo = new BaseRepository<Atraccion>(_context);
        _usuarioRepo = new UsuarioDbRepository(_context);
        _relojRepo = new RelojDbRepository(_context);

        _relojService = new RelojService(_relojRepo);

        var mockPluginManager = new MockPluginManager();
        _estrategiaService = new MockEstrategiaService(mockPluginManager);
        _service = new RankingService(_accesoRepo, _relojService, _estrategiaService);
    }

    [TestMethod]
    public void ObtenerEstrategiaActiva_PorDefecto_RetornaPuntuacionPorAtraccion()
    {
        IPuntuacion estrategia = _service.ObtenerEstrategiaActiva();

        Assert.IsInstanceOfType(estrategia, typeof(PuntuacionPorAtraccion));
        Assert.AreEqual("Puntuación Por Atracción", estrategia.Nombre);
    }

    [TestMethod]
    public void ObtenerRankingDiario_SinAccesos_DeberiaRetornarListaVacia()
    {
        IEnumerable<(Guid VisitanteId, string NombreVisitante, int PuntosTotales)> ranking =
            _service.ObtenerRankingDiario();

        Assert.AreEqual(0, ranking.Count());
    }

    [TestMethod]
    public void ObtenerRankingDiario_ConAccesos_DeberiaCalcularPuntosConEstrategiaActiva()
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

        var ranking = _service.ObtenerRankingDiario().ToList();

        Assert.AreEqual(2, ranking.Count);
        Assert.AreEqual(visitante1.Id, ranking[0].VisitanteId);
        Assert.AreEqual("Juan Perez", ranking[0].NombreVisitante);

        // ARREGLAR
        // Assert.AreEqual(175, ranking[0].PuntosTotales);
        Assert.AreEqual(visitante2.Id, ranking[1].VisitanteId);
        Assert.AreEqual(100, ranking[1].PuntosTotales);
    }

    [TestMethod]
    public void ObtenerRankingDiario_ConTopParameter_DeberiaLimitarResultados()
    {
        DateTime fechaHoy = _relojService.ObtenerFechaHora();

        for(var i = 0; i < 15; i++)
        {
            var visitante = new Visitante($"Visitante{i}", "Apellido", $"email{i}@test.com", "Pass1234!",
                new DateTime(2000, 1, 1), NivelMembresia.Estandar);
            _usuarioRepo.Agregar(visitante);
            var atraccion = new Atraccion($"Atraccion{i}", TipoAtraccion.MontañaRusa, 0, 50, "Desc", true);
            _atraccionRepo.Agregar(atraccion);
            var ticket = new Ticket(visitante, fechaHoy, TipoEntrada.General, null);
            _ticketRepo.Agregar(ticket);
            var acceso = new AccesoAtraccion(visitante, atraccion, ticket, fechaHoy);
            _accesoRepo.Agregar(acceso);
        }

        IEnumerable<(Guid VisitanteId, string NombreVisitante, int PuntosTotales)> ranking =
            _service.ObtenerRankingDiario(5);

        Assert.AreEqual(5, ranking.Count());
    }

    [TestMethod]
    public void ObtenerRankingDiario_CambioDeEstrategia_DeberiaRecalcularConNuevaEstrategia()
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

        (Guid VisitanteId, string NombreVisitante, int PuntosTotales) rankingAntes =
            _service.ObtenerRankingDiario().First();
        Assert.AreEqual(100, rankingAntes.PuntosTotales);

        _estrategiaService.EstablecerEstrategiaDirecta(new PuntuacionPorEvento(2.0m));
        (Guid VisitanteId, string NombreVisitante, int PuntosTotales) rankingDespues =
            _service.ObtenerRankingDiario().First();

        Assert.AreEqual(100, rankingDespues.PuntosTotales);
    }

    [TestMethod]
    public void ObtenerRankingDiario_SoloAccesosDelDia_DeberiaIgnorarOtrosDias()
    {
        _estrategiaService.EstablecerEstrategiaDirecta(new PuntuacionCombo());
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 0, 50, "Desc", true);
        _atraccionRepo.Agregar(atraccion);

        DateTime fechaHoy = _relojService.ObtenerFechaHora();
        DateTime fechaAyer = fechaHoy.AddDays(-1);

        var ticketHoy = new Ticket(visitante, fechaHoy, TipoEntrada.General, null);
        var ticketAyer = new Ticket(visitante, fechaAyer, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticketHoy);
        _ticketRepo.Agregar(ticketAyer);

        var accesoHoy = new AccesoAtraccion(visitante, atraccion, ticketHoy, fechaHoy);
        var accesoAyer = new AccesoAtraccion(visitante, atraccion, ticketAyer, fechaAyer);
        _accesoRepo.Agregar(accesoHoy);
        _accesoRepo.Agregar(accesoAyer);

        IEnumerable<(Guid VisitanteId, string NombreVisitante, int PuntosTotales)> ranking =
            _service.ObtenerRankingDiario();

        Assert.AreEqual(1, ranking.Count());
        Assert.AreEqual(100, ranking.First().PuntosTotales);
    }
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
    public void Inicializar(string rutaCarpetaPlugins)
    {
    }

    public IEnumerable<string> ObtenerNombresDeEstrategias()
    {
        return [];
    }

    public IPuntuacion? CrearInstancia(string nombreEstrategia)
    {
        return null;
    }
}
