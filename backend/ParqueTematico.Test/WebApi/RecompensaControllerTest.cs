using Dominio.Entities.Usuarios;
using Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Controllers;
namespace Test.WebApi;

[TestClass]
public class RecompensaControllerTest
{
    private RecompensaController _controller = null!;
    private Mock<IRecompensaService> _service = null!;

    [TestInitialize]
    public void SetUp()
    {
        _service = new Mock<IRecompensaService>();
        _controller = new RecompensaController(_service.Object);
    }

    [TestMethod]
    public void ObtenerTodos_RetornaCorrectamenteTodasLasRecompensas()
    {
        var lista = new List<RecompensaCreadoDto>();
        var dto = new RecompensaCreadoDto { Id = Guid.NewGuid(), Nombre = "Premio", Descripcion = "Test", Costo = 10 };
        lista.Add(dto);
        _service.Setup(s => s.ObtenerTodos()).Returns(lista);

        ActionResult<IList<RecompensaCreadoDto>> result = _controller.ObtenerTodos();

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(lista, okResult.Value);
    }

    [TestMethod]
    public void Obtener_RetornaRecurso()
    {
        var dto = new RecompensaCreadoDto { Id = Guid.NewGuid(), Nombre = "Premio", Descripcion = "Test", Costo = 10 };
        _service.Setup(s => s.ObtenerRecompensa(dto.Id)).Returns(dto);

        ActionResult<RecompensaCreadoDto> result = _controller.Obtener(dto.Id);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(dto, okResult.Value);
    }

    [TestMethod]
    public void Crear_DeberiaRetornarCreatedConUbicacion()
    {
        var dtoCrear =
            new RecompensaCrearDto { Nombre = "Nueva", Descripcion = "Des", Costo = 10, CantidadDisponible = 10 };
        var dtoCreado = new RecompensaCreadoDto
        {
            Id = Guid.NewGuid(),
            Nombre = "Nueva",
            Descripcion = "Des",
            Costo = 10,
            CantidadDisponible = 10
        };
        _service.Setup(s => s.CrearRecompensa(dtoCrear)).Returns(dtoCreado);

        ActionResult<RecompensaCreadoDto> result = _controller.Crear(dtoCrear);

        var createdResult = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(201, createdResult.StatusCode);
        Assert.AreEqual(dtoCreado, createdResult.Value);
        Assert.AreEqual(nameof(_controller.Obtener), createdResult.ActionName);
    }

    [TestMethod]
    public void Actualizar_DeberiaRetornarOkConRecompensaActualizada()
    {
        var dtoCreado = new RecompensaCreadoDto
        {
            Id = Guid.NewGuid(),
            Nombre = "Nueva",
            Descripcion = "Des",
            Costo = 10,
            CantidadDisponible = 10
        };

        var actualizada = new RecompensaCrearDto
        {
            Nombre = "Actualizado",
            Descripcion = "Des2",
            Costo = 1,
            CantidadDisponible = 1,
            NivelRequerido = NivelMembresia.Estandar
        };

        _service.Setup(s => s.ActualizarRecompensa(dtoCreado.Id, actualizada))
            .Returns(dtoCreado);

        ActionResult<RecompensaCreadoDto> result = _controller.Actualizar(dtoCreado.Id, actualizada);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        var returnedDto = okResult.Value as RecompensaCreadoDto;
        Assert.IsNotNull(returnedDto);
        Assert.AreEqual(dtoCreado.Id, returnedDto.Id);
    }

    [TestMethod]
    public void Eliminar_DeberiaRetornarNoContent()
    {
        var id = Guid.NewGuid();
        _service.Setup(s => s.EliminarRecompensa(id));

        IActionResult result = _controller.Eliminar(id);

        var noContent = result as NoContentResult;
        Assert.IsNotNull(noContent);
        Assert.AreEqual(204, noContent.StatusCode);
        _service.Verify(s => s.EliminarRecompensa(id), Times.Once);
    }
}
