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
public class RelojControllerTest
{
    private ParqueDbContext _context = null!;
    private RelojDbRepository _repo = null!;
    private RelojService _service = null!;

    [TestInitialize]
    public void SetUp()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _repo = new RelojDbRepository(_context);
        _service = new RelojService(_repo);
    }

    [TestMethod]
    public void RelojController_ParamsCorrectos_ControladorCreado()
    {
        var controller = new RelojController(_service);
        Assert.IsNotNull(controller);
    }

    [TestMethod]
    public void Put_RequestCorrecto_ActualizacionGuardada()
    {
        var controller = new RelojController(_service);
        var request = new RelojDto(DateTime.Now);
        ActionResult<RelojDto> response = controller.ModificarFechaHora(request);
        var okResult = response.Result as OkObjectResult;
        var relojResponse = okResult?.Value as RelojDto;

        Assert.IsNotNull(relojResponse, "Debe devlolver el reloj modificado");

        Reloj? relojGuardado = _repo.ObtenerPorId(1);
        Assert.IsNotNull(relojGuardado, "No existe el reloj");
        Assert.AreEqual(relojGuardado.FechaHora, relojResponse.FechaHora);
    }

    [TestMethod]
    public void Get_RequestCorrecto_ResponseCorrecto()
    {
        var controller = new RelojController(_service);
        ActionResult<RelojDto> response = controller.GetFechaHoraReloj();
        var okResult = response.Result as OkObjectResult;
        var relojResponse = okResult?.Value as RelojDto;

        Assert.IsNotNull(relojResponse, "Debe devlolver el reloj");
        Assert.AreEqual(relojResponse.FechaHora, _service.ObtenerFechaHora());
    }
}
