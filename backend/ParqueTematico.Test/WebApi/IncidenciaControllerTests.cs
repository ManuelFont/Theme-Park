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
public class IncidenciaControllerTests
{
    private IAtraccionRepository _atraccionRepo = null!;
    private AtraccionService _atraccionService = null!;
    private ParqueDbContext _context = null!;
    private IncidenciaDbRepository _incidenciaRepo = null!;
    private IncidenciaService _service = null!;

    [TestInitialize]
    public void SetUp()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _incidenciaRepo = new IncidenciaDbRepository(_context);
        _atraccionRepo = new AtraccionRepository(_context);
        _atraccionService = new AtraccionService(_atraccionRepo);
        _service = new IncidenciaService(_incidenciaRepo, _atraccionService);
    }

    [TestMethod]
    public void IncidenciaController_ParamsCorrectos_ControladorCreado()
    {
        var controller = new IncidenciaController(_service);
        Assert.IsNotNull(controller);
    }

    [TestMethod]
    public void Crear_ConRequestValido_DeberiaCrearIncidencia()
    {
        var controller = new IncidenciaController(_service);
        _atraccionService.Crear("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Desc", true);
        Atraccion atraccion = _atraccionRepo.ObtenerTodos().First();

        var request = new CrearIncidenciaRequest
        {
            AtraccionId = atraccion.Id,
            TipoIncidencia = TipoIncidencia.Mantenimiento,
            Descripcion = "Cambio de correas"
        };

        ActionResult<IncidenciaDto> result = controller.Crear(request);

        var createdResult = result.Result as CreatedResult;
        Assert.IsNotNull(createdResult);
        var dto = createdResult.Value as IncidenciaDto;
        Assert.IsNotNull(dto);
        Assert.AreEqual(TipoIncidencia.Mantenimiento, dto.TipoIncidencia);
        Assert.AreEqual("Cambio de correas", dto.Descripcion);
        Assert.IsTrue(dto.EstaActiva);
    }

    [TestMethod]
    public void Cerrar_IncidenciaExiste_DeberiaCerrarla()
    {
        var controller = new IncidenciaController(_service);
        _atraccionService.Crear("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Desc", true);
        Atraccion atraccion = _atraccionRepo.ObtenerTodos().First();
        Incidencia incidencia = _service.Crear(atraccion.Id, TipoIncidencia.Mantenimiento, "Test");

        IActionResult result = controller.Cerrar(incidencia.Id);

        var noContentResult = result as NoContentResult;
        Assert.IsNotNull(noContentResult);

        Incidencia? incidenciaCerrada = _incidenciaRepo.ObtenerPorId(incidencia.Id);
        Assert.IsFalse(incidenciaCerrada!.EstaActiva);
    }

    [TestMethod]
    public void ObtenerActivasPorAtraccion_ConActivasTrue_DeberiaRetornarActivas()
    {
        var controller = new IncidenciaController(_service);
        _atraccionService.Crear("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Desc", true);
        Atraccion atraccion = _atraccionRepo.ObtenerTodos().First();

        Incidencia inc1 = _service.Crear(atraccion.Id, TipoIncidencia.Mantenimiento, "Inc 1");
        Incidencia inc2 = _service.Crear(atraccion.Id, TipoIncidencia.Rota, "Inc 2");
        _service.Cerrar(inc2.Id);

        ActionResult<List<IncidenciaDto>> result = controller.ObtenerActivasPorAtraccion(atraccion.Id, true);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var lista = okResult.Value as List<IncidenciaDto>;
        Assert.IsNotNull(lista);
        Assert.AreEqual(1, lista.Count);
        Assert.AreEqual(inc1.Id, lista[0].Id);
    }

    [TestMethod]
    public void ObtenerActivasPorAtraccion_ConActivasFalse_DeberiaRetornarVacio()
    {
        var controller = new IncidenciaController(_service);
        _atraccionService.Crear("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Desc", true);
        Atraccion atraccion = _atraccionRepo.ObtenerTodos().First();
        _service.Crear(atraccion.Id, TipoIncidencia.Mantenimiento, "Inc 1");

        ActionResult<List<IncidenciaDto>> result = controller.ObtenerActivasPorAtraccion(atraccion.Id);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var lista = okResult.Value as List<IncidenciaDto>;
        Assert.IsNotNull(lista);
        Assert.AreEqual(0, lista.Count);
    }

    [TestMethod]
    public void ExisteActiva_ConIncidenciaActiva_DeberiaRetornarTrue()
    {
        var controller = new IncidenciaController(_service);
        _atraccionService.Crear("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Desc", true);
        Atraccion atraccion = _atraccionRepo.ObtenerTodos().First();
        _service.Crear(atraccion.Id, TipoIncidencia.Mantenimiento, "Test");

        ActionResult<bool> result = controller.ExisteActiva(atraccion.Id);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var existe = (bool)okResult.Value!;
        Assert.IsTrue(existe);
    }

    [TestMethod]
    public void ExisteActiva_SinIncidencias_DeberiaRetornarFalse()
    {
        var controller = new IncidenciaController(_service);
        _atraccionService.Crear("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Desc", true);
        Atraccion atraccion = _atraccionRepo.ObtenerTodos().First();

        ActionResult<bool> result = controller.ExisteActiva(atraccion.Id);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var existe = (bool)okResult.Value!;
        Assert.IsFalse(existe);
    }
}
