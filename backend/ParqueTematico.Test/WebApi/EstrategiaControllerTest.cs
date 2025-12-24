using Dominio.Entities.Puntuacion;
using Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Controllers;

namespace Test.WebApi;

[TestClass]
public class EstrategiaControllerTests
{
    private Mock<IEstrategiaService> _serviceMock = null!;
    private EstrategiaController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _serviceMock = new Mock<IEstrategiaService>();
        _controller = new EstrategiaController(_serviceMock.Object);
    }

    [TestMethod]
    public void ObtenerTodas_RetornaOkConLista()
    {
        // Arrange
        var mockPuntuacion = new Mock<IPuntuacion>();
        mockPuntuacion.SetupGet(p => p.Nombre).Returns("EstrategiaA");
        mockPuntuacion.SetupGet(p => p.Descripcion).Returns("Desc A");

        var estrategias = new[] { ("EstrategiaX", "Descripcion X") };

        _serviceMock
            .Setup(s => s.ObtenerEstrategiasDisponibles())
            .Returns(estrategias);

        var actionResult = _controller.ObtenerTodas();
        var result = actionResult.Result as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var lista = result.Value as IEnumerable<EstrategiaDto>;
        Assert.IsNotNull(lista);
        Assert.AreEqual(1, lista.Count());
    }

    [TestMethod]
    public void ObtenerActiva_RetornaOk()
    {
        var puntuacionMock = new Mock<IPuntuacion>();
        puntuacionMock.SetupGet(p => p.Descripcion).Returns("desc activa");

        _serviceMock
            .Setup(s => s.ObtenerEstrategiaActiva())
            .Returns(puntuacionMock.Object);

        var actionResult = _controller.ObtenerActiva();
        var result = actionResult.Result as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var dto = result.Value as EstrategiaDto;
        Assert.IsNotNull(dto);
        Assert.AreEqual("desc activa", dto.Descripcion);
        Assert.AreEqual(puntuacionMock.Object.GetType().Name, dto.Nombre);
    }

    [TestMethod]
    public void CambiarEstrategia_CuandoNoExiste_RetornaBadRequest()
    {
        var request = new CambiarEstrategiaRequest
        {
            NombreEstrategia = "Inexistente"
        };

        _serviceMock
            .Setup(s => s.CambiarEstrategia("Inexistente"))
            .Returns(false);

        var actionResult = _controller.CambiarEstrategia(request);
        var result = actionResult as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        Assert.IsTrue(result.Value!.ToString()!.Contains("Inexistente"));
    }

    [TestMethod]
    public void CambiarEstrategia_CuandoExiste_RetornaNoContent()
    {
        var request = new CambiarEstrategiaRequest
        {
            NombreEstrategia = "Valida"
        };

        _serviceMock
            .Setup(s => s.CambiarEstrategia("Valida"))
            .Returns(true);

        var actionResult = _controller.CambiarEstrategia(request);
        var result = actionResult as NoContentResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(204, result.StatusCode);
    }
}
