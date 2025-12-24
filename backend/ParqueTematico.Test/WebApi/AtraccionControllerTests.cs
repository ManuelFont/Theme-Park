using Dominio.Entities;
using Dtos;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ParqueTematico.Application.Services;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Controllers;
using RepositoryInterfaces;

namespace Test.WebApi;

[TestClass]
public class AtraccionControllerTests
{
    private ParqueDbContext _context = null!;
    private IAtraccionRepository _repo = null!;
    private AtraccionService _service = null!;
    private AtraccionController _controller = null!;
    private Mock<IAccesoAtraccionService> _accesoMock = null!;

    [TestInitialize]
    public void SetUp()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _repo = new AtraccionRepository(_context);
        _service = new AtraccionService(_repo);
        _accesoMock = new Mock<IAccesoAtraccionService>();
        _controller = new AtraccionController(_service, _accesoMock.Object);
    }

    [TestMethod]
    public void AtraccionController_ParamsCorrectos_ControladorCreado()
    {
        Assert.IsNotNull(_controller);
    }

    [TestMethod]
    public void Crear_ConDtoValido_DeberiaCrearAtraccion()
    {
        var dto = new AtraccionDto
        {
            Nombre = "Montaña Rusa",
            Tipo = TipoAtraccion.MontañaRusa,
            EdadMinima = 12,
            CapacidadMaxima = 30,
            Descripcion = "Emoción extrema"
        };

        IActionResult result = _controller.Crear(dto);

        var okResult = result as OkResult;
        Assert.IsNotNull(okResult);

        var atracciones = _repo.ObtenerTodos().ToList();
        Assert.AreEqual(1, atracciones.Count);
        Assert.AreEqual("Montaña Rusa", atracciones[0].Nombre);
    }

    [TestMethod]
    public void ObtenerPorId_AtraccionExiste_DeberiaRetornarDto()
    {
        _service.Crear("Simulador", TipoAtraccion.Simulador, 10, 20, "Desc", true);
        Atraccion atraccion = _repo.ObtenerTodos().First();

        ActionResult<AtraccionDto> result = _controller.ObtenerPorId(atraccion.Id);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var dto = okResult.Value as AtraccionDto;
        Assert.IsNotNull(dto);
        Assert.AreEqual(atraccion.Id, dto.Id);
        Assert.AreEqual("Simulador", dto.Nombre);
    }

    [TestMethod]
    public void ObtenerPorId_AtraccionNoExiste_DeberiaRetornarNotFound()
    {
        var id = Guid.NewGuid();

        ActionResult<AtraccionDto> result = _controller.ObtenerPorId(id);

        var notFoundResult = result.Result as NotFoundResult;
        Assert.IsNotNull(notFoundResult);
    }

    [TestMethod]
    public void ObtenerTodos_SinAtracciones_DeberiaRetornarListaVacia()
    {
        ActionResult<List<AtraccionDto>> result = _controller.ObtenerTodos();

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var lista = okResult.Value as List<AtraccionDto>;
        Assert.IsNotNull(lista);
        Assert.AreEqual(0, lista.Count);
    }

    [TestMethod]
    public void ObtenerTodos_ConAtracciones_DeberiaRetornarLista()
    {
        _service.Crear("Atraccion 1", TipoAtraccion.MontañaRusa, 10, 20, "Desc 1", true);
        _service.Crear("Atraccion 2", TipoAtraccion.Simulador, 12, 25, "Desc 2", true);

        ActionResult<List<AtraccionDto>> result = _controller.ObtenerTodos();

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var lista = okResult.Value as List<AtraccionDto>;
        Assert.IsNotNull(lista);
        Assert.AreEqual(2, lista.Count);
    }

    [TestMethod]
    public void Actualizar_AtraccionExiste_DeberiaActualizarla()
    {
        _service.Crear("Original", TipoAtraccion.MontañaRusa, 10, 20, "Desc", true);
        Atraccion atraccion = _repo.ObtenerTodos().First();

        var dto = new AtraccionDto
        {
            Nombre = "Actualizada",
            Tipo = TipoAtraccion.Simulador,
            EdadMinima = 15,
            CapacidadMaxima = 30,
            Descripcion = "Nueva desc"
        };

        IActionResult result = _controller.Actualizar(atraccion.Id, dto);

        var okResult = result as OkResult;
        Assert.IsNotNull(okResult);

        Atraccion? actualizada = _repo.ObtenerPorId(atraccion.Id);
        Assert.AreEqual("Actualizada", actualizada!.Nombre);
        Assert.AreEqual(TipoAtraccion.Simulador, actualizada.Tipo);
    }

    [TestMethod]
    public void Eliminar_AtraccionExiste_DeberiaEliminarla()
    {
        _service.Crear("A Eliminar", TipoAtraccion.MontañaRusa, 10, 20, "Desc", true);
        Atraccion atraccion = _repo.ObtenerTodos().First();

        IActionResult result = _controller.Eliminar(atraccion.Id);

        var okResult = result as OkResult;
        Assert.IsNotNull(okResult);

        var atracciones = _repo.ObtenerTodos().ToList();
        Assert.AreEqual(0, atracciones.Count);
    }

    [TestMethod]
    public void ObtenerAforo_AtraccionNoExiste_DeberiaRetornarNotFound()
    {
        var id = Guid.NewGuid();

        ActionResult<AforoDto> result = _controller.ObtenerAforo(id);

        var notFound = result.Result as NotFoundObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual("La atracción no existe", notFound.Value);
    }

    [TestMethod]
    public void ObtenerAforo_AtraccionExiste_AforoCero_DeberiaRetornarDtoCorrecto()
    {
        _service.Crear("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Desc", true);
        var atraccion = _repo.ObtenerTodos().First();

        _accesoMock
            .Setup(a => a.ObtenerAforoActual(atraccion.Id))
            .Returns(0);

        ActionResult<AforoDto> result = _controller.ObtenerAforo(atraccion.Id);

        var ok = result.Result as OkObjectResult;
        Assert.IsNotNull(ok);

        var dto = ok.Value as AforoDto;
        Assert.IsNotNull(dto);

        Assert.AreEqual(atraccion.Id, dto.AtraccionId);
        Assert.AreEqual(30, dto.CapacidadMaxima);
        Assert.AreEqual(0, dto.AforoActual);
        Assert.AreEqual(30, dto.Restante);
    }
}
