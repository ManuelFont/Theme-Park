using Dominio.Entities;
using Dtos;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using ParqueTematico.Application.Services;
using ParqueTematico.WebApi.Controllers;

namespace Test.WebApi;

[TestClass]
public class EventoControllerTests
{
    private ParqueDbContext _context = null!;
    private EventoDbRepository _repo = null!;
    private EventoService _service = null!;

    [TestInitialize]
    public void SetUp()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _repo = new EventoDbRepository(_context);
        var atraccionRepo = new AtraccionRepository(_context);
        var atraccionService = new AtraccionService(atraccionRepo);
        _service = new EventoService(_repo, atraccionService);
    }

    [TestMethod]
    public void EventoController_ParamsCorrectos_ControladorCreado()
    {
        var controller = new EventoController(_service);
        Assert.IsNotNull(controller);
    }

    [TestMethod]
    public void Crear_ConDtoValido_DeberiaCrearEvento()
    {
        var controller = new EventoController(_service);
        var dto = new CrearEventoRequest()
        {
            Nombre = "Noche de Dinosaurios",
            Fecha = new DateTime(2026, 10, 15),
            Hora = new TimeSpan(20, 0, 0),
            Aforo = 100,
            CostoAdicional = 500,
        };

        IActionResult result = controller.Crear(dto);

        var createdResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);

        var eventos = _repo.ObtenerTodos().ToList();
        Assert.AreEqual(1, eventos.Count);
        Assert.AreEqual("Noche de Dinosaurios", eventos[0].Nombre);
    }

    [TestMethod]
    public void Eliminar_EventoExiste_DeberiaEliminarlo()
    {
        var controller = new EventoController(_service);
        _service.CrearEvento("Evento a Eliminar", new DateTime(2026, 11, 1), new TimeSpan(19, 0, 0), 50, 300);
        Evento evento = _repo.ObtenerTodos().First();

        IActionResult result = controller.Eliminar(evento.Id);

        var noContentResult = result as NoContentResult;
        Assert.IsNotNull(noContentResult);

        var eventos = _repo.ObtenerTodos().ToList();
        Assert.AreEqual(0, eventos.Count);
    }

    [TestMethod]
    public void ObtenerTodos_SinEventos_DeberiaRetornarListaVacia()
    {
        var controller = new EventoController(_service);

        ActionResult<List<EventoDto>> result = controller.ObtenerTodos();

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var lista = okResult.Value as List<EventoDto>;
        Assert.IsNotNull(lista);
        Assert.AreEqual(0, lista.Count);
    }

    [TestMethod]
    public void ObtenerTodos_ConEventos_DeberiaRetornarLista()
    {
        var controller = new EventoController(_service);
        _service.CrearEvento("Evento 1", new DateTime(2026, 10, 15), new TimeSpan(20, 0, 0), 100, 500);
        _service.CrearEvento("Evento 2", new DateTime(2026, 11, 20), new TimeSpan(21, 0, 0), 150, 600);

        ActionResult<List<EventoDto>> result = controller.ObtenerTodos();

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var lista = okResult.Value as List<EventoDto>;
        Assert.IsNotNull(lista);
        Assert.AreEqual(2, lista.Count);
    }

    [TestMethod]
    public void AgregarAtraccion_DatosValidos_RetornaOk()
    {
        var atraccionRepo = new BaseRepository<Atraccion>(_context);
        var controller = new EventoController(_service);

        var atraccion = new Atraccion("Casa del Terror", TipoAtraccion.Simulador, 10, 20, "Atracción de miedo", true);
        atraccionRepo.Agregar(atraccion);
        _service.CrearEvento("Halloween Night", new DateTime(2026, 10, 31), new TimeSpan(22, 0, 0), 80, 400);
        Evento evento = _repo.ObtenerTodos().First();

        IActionResult result = controller.AgregarAtraccion(evento.Id, atraccion.Id);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var value = okResult.Value;
        Assert.IsNotNull(value);

        Evento? eventoActualizado = _repo.ObtenerPorId(evento.Id);
        Assert.IsTrue(eventoActualizado!.Atracciones.Any(a => a.Id == atraccion.Id));
    }

    [TestMethod]
    public void EliminarAtraccion_DatosValidos_RetornaOk()
    {
        var atraccionRepo = new BaseRepository<Atraccion>(_context);
        var controller = new EventoController(_service);

        var atraccion = new Atraccion("Montaña Rusa Nocturna", TipoAtraccion.MontañaRusa, 10, 50, "Atracción nocturna", true);
        atraccionRepo.Agregar(atraccion);
        _service.CrearEvento("Festival Nocturno", new DateTime(2025, 12, 15), new TimeSpan(21, 0, 0), 100, 800);
        Evento evento = _repo.ObtenerTodos().First();

        _service.AgregarAtraccionAEvento(evento.Id, atraccion.Id);

        IActionResult result = controller.EliminarAtraccion(evento.Id, atraccion.Id);

        var noContentResult = result as NoContentResult;
        Assert.IsNotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);

        Evento? eventoActualizado = _repo.ObtenerPorId(evento.Id);
        Assert.IsFalse(eventoActualizado!.Atracciones.Any(a => a.Id == atraccion.Id));
    }

    [TestMethod]
    public void ObtenerPorId_CuandoNoExiste_DeberiaRetornarNotFound()
    {
        var controller = new EventoController(_service);
        var idInexistente = Guid.NewGuid();

        ActionResult<EventoDto> result = controller.ObtenerPorId(idInexistente);

        var notFound = result.Result as NotFoundResult;
        Assert.IsNotNull(notFound);
    }

    [TestMethod]
    public void ObtenerPorId_CuandoExiste_DeberiaRetornarOkConDto()
    {
        var controller = new EventoController(_service);

        _service.CrearEvento(
            "Evento Test",
            new DateTime(2026, 12, 25),
            new TimeSpan(20, 0, 0),
            120,
            450);

        Evento evento = _repo.ObtenerTodos().First();

        ActionResult<EventoDto> result = controller.ObtenerPorId(evento.Id);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var dto = okResult.Value as EventoDto;
        Assert.IsNotNull(dto);
        Assert.AreEqual(evento.Id, dto.Id);
        Assert.AreEqual(evento.Nombre, dto.Nombre);
    }
}
