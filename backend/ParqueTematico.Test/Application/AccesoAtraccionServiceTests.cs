using Dominio.Entities;
using Dominio.Entities.Puntuacion;
using Dominio.Entities.Usuarios;
using Dominio.Exceptions;
using Dtos;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using ParqueTematico.Application.Services;
using ParqueTematico.BusinessLogicInterface;

namespace Test.Application;

[TestClass]
public class AccesoAtraccionServiceTests
{
    private AccesoAtraccionDbRepository _accesoRepo = null!;
    private BaseRepository<Atraccion> _atraccionRepo = null!;
    private AtraccionService _atraccionService = null!;
    private ParqueDbContext _context = null!;
    private EventoDbRepository _eventoRepo = null!;
    private EventoService _eventoService = null!;
    private HistorialPuntuacionDbRepository _historialRepo = null!;
    private HistorialPuntuacionService _historialService = null!;
    private IncidenciaDbRepository _incidenciaRepo = null!;
    private IncidenciaService _incidenciaService = null!;
    private RankingService _rankingService = null!;
    private RelojDbRepository _relojRepo = null!;
    private RelojService _relojService = null!;
    private AccesoAtraccionService _service = null!;
    private TicketDbRepository _ticketRepo = null!;
    private TicketService _ticketService = null!;
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
        _eventoRepo = new EventoDbRepository(_context);
        _incidenciaRepo = new IncidenciaDbRepository(_context);
        _relojRepo = new RelojDbRepository(_context);
        _historialRepo = new HistorialPuntuacionDbRepository(_context);

        _relojService = new RelojService(_relojRepo);
        _atraccionService = new AtraccionService(_atraccionRepo);
        _eventoService = new EventoService(_eventoRepo, _atraccionService);
        _ticketService = new TicketService(_ticketRepo, _usuarioRepo, _eventoService, _relojService);
        _incidenciaService = new IncidenciaService(_incidenciaRepo, _atraccionService);
        _historialService = new HistorialPuntuacionService(_historialRepo);

        var mockPluginManager = new MockPluginManager();
        _estrategiaService = new MockEstrategiaService(mockPluginManager);
        _rankingService = new RankingService(_accesoRepo, _relojService, _estrategiaService);

        _service = new AccesoAtraccionService(_accesoRepo, _ticketService, _atraccionService, _relojService,
            _incidenciaService, _rankingService, _historialService, _usuarioRepo);
    }

    [TestMethod]
    public void RegistrarIngreso_DatosValidos_DeberiaCrearAcceso()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        _atraccionRepo.Agregar(atraccion);
        DateTime fechaHoy = _relojService.ObtenerFechaHora();
        var ticket = new Ticket(visitante, fechaHoy, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticket);

        Guid accesoId = _service.RegistrarIngreso(ticket.Id, atraccion.Id);

        AccesoAtraccion? acceso = _accesoRepo.ObtenerPorId(accesoId);
        Assert.IsNotNull(acceso);
        Assert.AreEqual(visitante.Id, acceso.Visitante.Id);
        Assert.AreEqual(atraccion.Id, acceso.Atraccion.Id);
        Assert.AreEqual(atraccion.Disponible, true);
        Assert.IsNull(acceso.FechaHoraEgreso);
    }

    [TestMethod]
    [ExpectedException(typeof(AccesoAtraccionException))]
    public void RegistrarIngreso_TicketInexistente_DeberiaLanzarExcepcion()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        _atraccionRepo.Agregar(atraccion);

        _service.RegistrarIngreso(Guid.NewGuid(), atraccion.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(AccesoAtraccionException))]
    public void RegistrarIngreso_AtraccionInexistente_DeberiaLanzarExcepcion()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        DateTime fechaHoy = _relojService.ObtenerFechaHora();
        var ticket = new Ticket(visitante, fechaHoy, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticket);

        _service.RegistrarIngreso(ticket.Id, Guid.NewGuid());
    }

    [TestMethod]
    [ExpectedException(typeof(AccesoAtraccionException))]
    public void RegistrarIngreso_FechaTicketDiferente_DeberiaLanzarExcepcion()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        _atraccionRepo.Agregar(atraccion);
        DateTime fechaManana = _relojService.ObtenerFechaHora().AddDays(1);
        var ticket = new Ticket(visitante, fechaManana, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticket);

        _service.RegistrarIngreso(ticket.Id, atraccion.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(AccesoAtraccionException))]
    public void RegistrarIngreso_VisitanteMenorDeEdad_DeberiaLanzarExcepcion()
    {
        DateTime fechaHoy = _relojService.ObtenerFechaHora();
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", fechaHoy.AddYears(-10),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        _atraccionRepo.Agregar(atraccion);
        var ticket = new Ticket(visitante, fechaHoy, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticket);

        _service.RegistrarIngreso(ticket.Id, atraccion.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(AccesoAtraccionException))]
    public void RegistrarIngreso_AtraccionConIncidencia_DeberiaLanzarExcepcion()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        _atraccionRepo.Agregar(atraccion);
        _incidenciaService.Crear(atraccion.Id, TipoIncidencia.FueraDeServicio, "Mantenimiento");
        DateTime fechaHoy = _relojService.ObtenerFechaHora();
        var ticket = new Ticket(visitante, fechaHoy, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticket);

        _service.RegistrarIngreso(ticket.Id, atraccion.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(AccesoAtraccionException))]
    public void RegistrarIngreso_AforoCompleto_DeberiaLanzarExcepcion()
    {
        var visitante1 = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var visitante2 = new Visitante("Maria", "Lopez", "maria@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante1);
        _usuarioRepo.Agregar(visitante2);
        var atraccion = new Atraccion("Simulador", TipoAtraccion.Simulador, 0, 1, "Descripcion", true);
        _atraccionRepo.Agregar(atraccion);
        DateTime fechaHoy = _relojService.ObtenerFechaHora();
        var ticket1 = new Ticket(visitante1, fechaHoy, TipoEntrada.General, null);
        var ticket2 = new Ticket(visitante2, fechaHoy, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticket1);
        _ticketRepo.Agregar(ticket2);

        _service.RegistrarIngreso(ticket1.Id, atraccion.Id);
        _service.RegistrarIngreso(ticket2.Id, atraccion.Id);
    }

    [TestMethod]
    public void RegistrarEgreso_AccesoValido_DeberiaActualizarFecha()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        _atraccionRepo.Agregar(atraccion);
        DateTime fechaHoy = _relojService.ObtenerFechaHora();
        var ticket = new Ticket(visitante, fechaHoy, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticket);
        Guid accesoId = _service.RegistrarIngreso(ticket.Id, atraccion.Id);

        _service.RegistrarEgreso(accesoId);

        AccesoAtraccion? acceso = _accesoRepo.ObtenerPorId(accesoId);
        Assert.IsNotNull(acceso);
        Assert.IsNotNull(acceso.FechaHoraEgreso);
    }

    [TestMethod]
    public void AsignarPuntos_AccesoValido_DeberiaActualizarPuntos()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        _atraccionRepo.Agregar(atraccion);
        DateTime fechaHoy = _relojService.ObtenerFechaHora();
        var ticket = new Ticket(visitante, fechaHoy, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticket);
        Guid accesoId = _service.RegistrarIngreso(ticket.Id, atraccion.Id);

        _service.AsignarPuntos(accesoId, 100);

        AccesoAtraccion? acceso = _accesoRepo.ObtenerPorId(accesoId);
        Assert.IsNotNull(acceso);
        Assert.AreEqual(100, acceso.PuntosObtenidos);
    }

    [TestMethod]
    [ExpectedException(typeof(AccesoAtraccionException))]
    public void RegistrarEgreso_AccesoInexistente_DeberiaLanzarExcepcion()
    {
        _service.RegistrarEgreso(Guid.NewGuid());
    }

    [TestMethod]
    public void ObtenerAforoActual_DeberiaRetornarCantidadAccesosActivos()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        _atraccionRepo.Agregar(atraccion);
        DateTime fechaHoy = _relojService.ObtenerFechaHora();
        var ticket = new Ticket(visitante, fechaHoy, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticket);
        _service.RegistrarIngreso(ticket.Id, atraccion.Id);

        var aforo = _service.ObtenerAforoActual(atraccion.Id);

        Assert.AreEqual(1, aforo);
    }

    [TestMethod]
    [ExpectedException(typeof(AccesoAtraccionException))]
    public void AsignarPuntos_AccesoInexistente_DeberiaLanzarExcepcion()
    {
        _service.AsignarPuntos(Guid.NewGuid(), 100);
    }

    [TestMethod]
    public void ObtenerReporteUsoAtracciones_SinAccesos_DeberiaRetornarListaVacia()
    {
        DateTime fechaInicio = DateTime.Now.AddDays(-7);
        DateTime fechaFin = DateTime.Now;

        List<ReporteUsoAtraccionDto> reporte = _service.ObtenerReporteUsoAtracciones(fechaInicio, fechaFin);

        Assert.AreEqual(0, reporte.Count);
    }

    [TestMethod]
    public void ObtenerReporteUsoAtracciones_ConUnaAtraccion_DeberiaRetornarReporte()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        _atraccionRepo.Agregar(atraccion);
        DateTime fechaHoy = _relojService.ObtenerFechaHora();
        var ticket = new Ticket(visitante, fechaHoy, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticket);
        _service.RegistrarIngreso(ticket.Id, atraccion.Id);

        List<ReporteUsoAtraccionDto> reporte =
            _service.ObtenerReporteUsoAtracciones(fechaHoy.AddDays(-1), fechaHoy.AddDays(1));

        Assert.AreEqual(1, reporte.Count);
        Assert.AreEqual(atraccion.Id, reporte[0].AtraccionId);
        Assert.AreEqual("Montaña Rusa", reporte[0].NombreAtraccion);
        Assert.AreEqual(1, reporte[0].CantidadVisitas);
    }

    [TestMethod]
    public void ObtenerReporteUsoAtracciones_ConVariasAtracciones_DeberiaOrdenarPorVisitas()
    {
        var visitante1 = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var visitante2 = new Visitante("Maria", "Lopez", "maria@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var visitante3 = new Visitante("Pedro", "Garcia", "pedro@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante1);
        _usuarioRepo.Agregar(visitante2);
        _usuarioRepo.Agregar(visitante3);

        var atraccion1 = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        var atraccion2 = new Atraccion("Simulador VR", TipoAtraccion.Simulador, 3, 25, "Descripcion", true);
        _atraccionRepo.Agregar(atraccion1);
        _atraccionRepo.Agregar(atraccion2);

        DateTime fechaHoy = _relojService.ObtenerFechaHora();
        var ticket1 = new Ticket(visitante1, fechaHoy, TipoEntrada.General, null);
        var ticket2 = new Ticket(visitante2, fechaHoy, TipoEntrada.General, null);
        var ticket3 = new Ticket(visitante3, fechaHoy, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticket1);
        _ticketRepo.Agregar(ticket2);
        _ticketRepo.Agregar(ticket3);

        _service.RegistrarIngreso(ticket1.Id, atraccion1.Id);
        _service.RegistrarIngreso(ticket2.Id, atraccion2.Id);
        _service.RegistrarIngreso(ticket3.Id, atraccion2.Id);

        List<ReporteUsoAtraccionDto> reporte =
            _service.ObtenerReporteUsoAtracciones(fechaHoy.AddDays(-1), fechaHoy.AddDays(1));

        Assert.AreEqual(2, reporte.Count);
        Assert.AreEqual("Simulador VR", reporte[0].NombreAtraccion);
        Assert.AreEqual(2, reporte[0].CantidadVisitas);
        Assert.AreEqual("Montaña Rusa", reporte[1].NombreAtraccion);
        Assert.AreEqual(1, reporte[1].CantidadVisitas);
    }

    [TestMethod]
    public void ObtenerReporteUsoAtracciones_MismaAtraccionVariasVisitas_DeberiaSumarTodas()
    {
        var visitante1 = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var visitante2 = new Visitante("Maria", "Lopez", "maria@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante1);
        _usuarioRepo.Agregar(visitante2);

        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        _atraccionRepo.Agregar(atraccion);

        DateTime fechaHoy = _relojService.ObtenerFechaHora();
        var ticket1 = new Ticket(visitante1, fechaHoy, TipoEntrada.General, null);
        var ticket2 = new Ticket(visitante2, fechaHoy, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticket1);
        _ticketRepo.Agregar(ticket2);

        _service.RegistrarIngreso(ticket1.Id, atraccion.Id);
        _service.RegistrarIngreso(ticket2.Id, atraccion.Id);

        List<ReporteUsoAtraccionDto> reporte =
            _service.ObtenerReporteUsoAtracciones(fechaHoy.AddDays(-1), fechaHoy.AddDays(1));

        Assert.AreEqual(1, reporte.Count);
        Assert.AreEqual(atraccion.Id, reporte[0].AtraccionId);
        Assert.AreEqual(2, reporte[0].CantidadVisitas);
    }

    [TestMethod]
    public void ObtenerAccesosPorVisitanteYFecha_DeberiaRetornarAccesos()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        _atraccionRepo.Agregar(atraccion);
        DateTime fechaHoy = _relojService.ObtenerFechaHora();
        var ticket = new Ticket(visitante, fechaHoy, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticket);
        _service.RegistrarIngreso(ticket.Id, atraccion.Id);

        IEnumerable<AccesoAtraccion> accesos = _service.ObtenerAccesosPorVisitanteYFecha(visitante.Id, fechaHoy);

        Assert.AreEqual(1, accesos.Count());
    }

    [TestMethod]
    public void RegistrarEgreso_DeberiaRegistrarHistorialAutomaticamente()
    {
        var visitante = new Visitante("Maria", "Lopez", "maria@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        _atraccionRepo.Agregar(atraccion);
        DateTime fechaHoy = _relojService.ObtenerFechaHora();
        var ticket = new Ticket(visitante, fechaHoy, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticket);

        Guid accesoId = _service.RegistrarIngreso(ticket.Id, atraccion.Id);
        _service.RegistrarEgreso(accesoId);

        IList<HistorialPuntuacion> historial = _historialService.ObtenerHistorialPorVisitante(visitante.Id);

        Assert.AreEqual(1, historial.Count);
        Assert.AreEqual(100, historial[0].Puntos);
        Assert.AreEqual("Montaña Rusa", historial[0].Origen);
        Assert.AreEqual("Puntuación Por Atracción", historial[0].EstrategiaActiva);
    }

    [TestMethod]
    public void RegistrarEgreso_DeberiaAsignarPuntosAlVisitante()
    {
        var visitante = new Visitante("Juan", "Garcia", "juan@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", true);
        _atraccionRepo.Agregar(atraccion);
        DateTime fechaHoy = _relojService.ObtenerFechaHora();
        var ticket = new Ticket(visitante, fechaHoy, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticket);

        Assert.AreEqual(0, visitante.PuntosActuales);

        Guid accesoId = _service.RegistrarIngreso(ticket.Id, atraccion.Id);
        _service.RegistrarEgreso(accesoId);

        var visitanteActualizado = (Visitante?)_usuarioRepo.ObtenerPorId(visitante.Id);
        Assert.IsNotNull(visitanteActualizado);
        Assert.AreEqual(100, visitanteActualizado.PuntosActuales);
    }

    [TestMethod]
    public void RegistrarEgreso_ConEstrategiaCombo_DeberiaUsarEstrategiaActiva()
    {
        var visitante = new Visitante("Pedro", "Garcia", "pedro@test.com", "Pass1234!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(visitante);
        var atraccion = new Atraccion("Simulador", TipoAtraccion.Simulador, 0, 50, "Descripcion", true);
        _atraccionRepo.Agregar(atraccion);
        DateTime fechaHoy = _relojService.ObtenerFechaHora();
        var ticket = new Ticket(visitante, fechaHoy, TipoEntrada.General, null);
        _ticketRepo.Agregar(ticket);

        _estrategiaService.EstablecerEstrategiaDirecta(new PuntuacionCombo());

        Guid accesoId = _service.RegistrarIngreso(ticket.Id, atraccion.Id);
        _service.RegistrarEgreso(accesoId);

        IList<HistorialPuntuacion> historial = _historialService.ObtenerHistorialPorVisitante(visitante.Id);

        Assert.AreEqual(1, historial.Count);
        Assert.AreEqual("Puntuación Combo", historial[0].EstrategiaActiva);
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
            return null;
        }

        public IEnumerable<string> ObtenerNombresDeEstrategias()
        {
            return [];
        }
    }
}
