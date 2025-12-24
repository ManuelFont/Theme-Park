using Dominio.Entities;
using Dtos;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using ParqueTematico.Application.Services;
using ParqueTematico.WebApi.Controllers;
using RepositoryInterfaces;

namespace Test.WebApi;

[TestClass]
public class MantenimientoPreventivoControllerTests
{
    private IAtraccionRepository _atraccionRepo = null!;
    private AtraccionService _atraccionService = null!;
    private ParqueDbContext _context = null!;
    private MantenimientoPreventivoController _controller = null!;
    private RelojService _reloj = null!;
    private RelojDbRepository _relojRepo = null!;
    private IncidenciaDbRepository _repo = null!;
    private MantenimientoPreventivoService _service = null!;
    private MantenimientoPreventivoDbRepository _mantenimientoPreventivoRepo = null!;

    [TestInitialize]
    public void SetUp()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _repo = new IncidenciaDbRepository(_context);
        _atraccionRepo = new AtraccionRepository(_context);
        _atraccionService = new AtraccionService(_atraccionRepo);
        _relojRepo = new RelojDbRepository(_context);
        _reloj = new RelojService(_relojRepo);
        _mantenimientoPreventivoRepo = new MantenimientoPreventivoDbRepository(_context);
        _service = new MantenimientoPreventivoService(_mantenimientoPreventivoRepo, _reloj, _atraccionService);
        _controller = new MantenimientoPreventivoController(_service);
    }

    [TestMethod]
    public void Crear_RequestCorrecto_201MantenimientoCreado()
    {
        _atraccionService.Crear("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Desc", true);
        Atraccion atraccion = _atraccionRepo.ObtenerTodos().First();

        var request = new CrearMantenimientoPreventivoRequest()
        {
            AtraccionId = atraccion.Id,
            Descripcion = "Cambio de correas",
            FechaInicio = new DateTime(2026, 1, 1),
            FechaFin = new DateTime(2026, 1, 2)
        };

        var result = _controller.CrearMantenimiento(request).Result as CreatedAtActionResult;
        var dto = result?.Value as MantenimientoPreventivoDto;
        Assert.AreEqual(request.Descripcion, dto?.Descripcion);
        if(dto != null)
        {
            Assert.AreEqual(request.AtraccionId, dto.AtraccionId);
            Assert.AreEqual(request.FechaInicio, dto.FechaInicio);
            Assert.AreEqual(request.FechaFin, dto.FechaFin);
        }
    }

    [TestMethod]
    public void ObtenerPorId_MantenimientoExiste_DebeRetornarMantenimiento()
    {
        _atraccionService.Crear("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Desc", true);
        Atraccion atraccion = _atraccionRepo.ObtenerTodos().First();

        var request = new CrearMantenimientoPreventivoRequest()
        {
            AtraccionId = atraccion.Id,
            Descripcion = "Cambio de correas",
            FechaInicio = new DateTime(2026, 1, 1),
            FechaFin = new DateTime(2026, 1, 2)
        };

        var createResult = _controller.CrearMantenimiento(request).Result as CreatedAtActionResult;
        var createdDto = createResult?.Value as MantenimientoPreventivoDto;

        var result = _controller.ObtenerPorId(createdDto!.Id);
        var okResult = result.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        var dto = okResult.Value as MantenimientoPreventivoDto;
        Assert.IsNotNull(dto);
        Assert.AreEqual(createdDto.Id, dto.Id);
        Assert.AreEqual("Cambio de correas", dto.Descripcion);
    }

    [TestMethod]
    public void ObtenerPorId_MantenimientoNoExiste_DebeRetornarNotFound()
    {
        var result = _controller.ObtenerPorId(Guid.NewGuid());

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ObtenerTodos_SinMantenimientos_DebeRetornarListaVacia()
    {
        var result = _controller.ObtenerTodos();
        var okResult = result.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        var lista = okResult.Value as IEnumerable<MantenimientoPreventivoDto>;
        Assert.IsNotNull(lista);
        Assert.AreEqual(0, lista.Count());
    }

    [TestMethod]
    public void ObtenerTodos_ConMantenimientos_DebeRetornarLista()
    {
        _atraccionService.Crear("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Desc", true);
        Atraccion atraccion = _atraccionRepo.ObtenerTodos().First();

        var request = new CrearMantenimientoPreventivoRequest()
        {
            AtraccionId = atraccion.Id,
            Descripcion = "Cambio de correas",
            FechaInicio = new DateTime(2026, 1, 1),
            FechaFin = new DateTime(2026, 1, 2)
        };

        _controller.CrearMantenimiento(request);

        var result = _controller.ObtenerTodos();
        var okResult = result.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        var lista = okResult.Value as IEnumerable<MantenimientoPreventivoDto>;
        Assert.IsNotNull(lista);
        Assert.AreEqual(1, lista.Count());
    }

    [TestMethod]
    public void Finalizar_MantenimientoExiste_DebeRetornarNoContent()
    {
        _atraccionService.Crear("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Desc", true);
        Atraccion atraccion = _atraccionRepo.ObtenerTodos().First();

        var request = new CrearMantenimientoPreventivoRequest()
        {
            AtraccionId = atraccion.Id,
            Descripcion = "Cambio de correas",
            FechaInicio = new DateTime(2026, 1, 1),
            FechaFin = new DateTime(2026, 1, 2)
        };

        var createResult = _controller.CrearMantenimiento(request).Result as CreatedAtActionResult;
        var createdDto = createResult?.Value as MantenimientoPreventivoDto;

        var result = _controller.Finalizar(createdDto!.Id);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }
}
