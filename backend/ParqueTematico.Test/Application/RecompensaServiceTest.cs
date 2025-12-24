using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Dominio.Exceptions;
using Dtos;
using Moq;
using ParqueTematico.Application.Services;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace Test.Application;

[TestClass]
public class RecompensaServiceTest
{
    private IRecompensaService _recompensaService = null!;
    private Mock<IBaseRepository<Recompensa>> _repoRecompensa = null!;

    [TestInitialize]
    public void SetUp()
    {
        _repoRecompensa = new Mock<IBaseRepository<Recompensa>>();
        _recompensaService = new RecompensaService(_repoRecompensa.Object);
    }

    [TestMethod]
    public void CrearRecompensa_ValorCorrectos_RecompensaCreada()
    {
        var crearDto = new RecompensaCrearDto
        {
            Nombre = "Tele",
            Descripcion = "40 pulgadas",
            CantidadDisponible = 10,
            Costo = 10,
            NivelRequerido = NivelMembresia.Estandar
        };

        RecompensaCreadoDto creadoDto = _recompensaService.CrearRecompensa(crearDto);

        Assert.IsNotNull(creadoDto.Id);
        Assert.AreEqual(crearDto.Nombre, creadoDto.Nombre);
        Assert.AreEqual(creadoDto.Descripcion, crearDto.Descripcion);
        Assert.AreEqual(creadoDto.Costo, crearDto.Costo);
        Assert.AreEqual(crearDto.CantidadDisponible, creadoDto.CantidadDisponible);
        Assert.AreEqual(crearDto.NivelRequerido, creadoDto.NivelRequerido);
    }

    [TestMethod]
    public void ObtenerTodos_HayRecompensas_RetornaListaRecompensas()
    {
        var recompensa = new Recompensa("TV", "descripcion", 100, 10, NivelMembresia.Estandar);
        _repoRecompensa.Setup(r => r.ObtenerTodos()).Returns([recompensa]);

        Assert.IsNotNull(_recompensaService.ObtenerTodos().FirstOrDefault());
        Assert.AreEqual(recompensa.Nombre, _recompensaService.ObtenerTodos().FirstOrDefault()!.Nombre);
        Assert.AreEqual(recompensa.Descripcion, _recompensaService.ObtenerTodos().FirstOrDefault()!.Descripcion);
        Assert.AreEqual(recompensa.Costo, _recompensaService.ObtenerTodos().FirstOrDefault()!.Costo);
        Assert.AreEqual(recompensa.CantidadDisponible,
            _recompensaService.ObtenerTodos().FirstOrDefault()!.CantidadDisponible);
        Assert.AreEqual(recompensa.NivelRequerido, _recompensaService.ObtenerTodos().FirstOrDefault()!.NivelRequerido);
    }

    [TestMethod]
    public void ObtenerRecompensa_Existe_RetornaDtoCorrecto()
    {
        var recompensa = new Recompensa("TV", "descripcion", 100, 10, NivelMembresia.Estandar);
        _repoRecompensa.Setup(r => r.ObtenerPorId(recompensa.Id)).Returns(recompensa);
        RecompensaCreadoDto dto = _recompensaService.ObtenerRecompensa(recompensa.Id);

        Assert.IsNotNull(dto);
        Assert.AreEqual(recompensa.Nombre, dto.Nombre);
        Assert.AreEqual(recompensa.Descripcion, dto.Descripcion);
        Assert.AreEqual(recompensa.Costo, dto.Costo);
        Assert.AreEqual(recompensa.CantidadDisponible, dto.CantidadDisponible);
        Assert.AreEqual(recompensa.NivelRequerido, dto.NivelRequerido);
    }

    [TestMethod]
    [ExpectedException(typeof(RecompensaNoEncontradaException))]
    public void ObtenerRecompensa_NoExiste_Error()
    {
        _recompensaService.ObtenerRecompensa(Guid.NewGuid());
    }

    [TestMethod]
    public void ActualizarRecompensa_Existe_Actualizado()
    {
        var recompensa = new Recompensa("TV", "descripcion", 100, 10, NivelMembresia.Estandar);
        _repoRecompensa.Setup(r => r.ObtenerPorId(recompensa.Id)).Returns(recompensa);
        RecompensaCreadoDto dto = _recompensaService.ObtenerRecompensa(recompensa.Id);
        var dtoCrear = new RecompensaCrearDto
        {
            Nombre = "Television",
            Descripcion = dto.Descripcion,
            Costo = dto.Costo,
            CantidadDisponible = dto.CantidadDisponible,
            NivelRequerido = dto.NivelRequerido
        };
        _recompensaService.ActualizarRecompensa(dto.Id, dtoCrear);

        Assert.AreEqual("Television", _recompensaService.ObtenerRecompensa(recompensa.Id).Nombre);
    }

    [TestMethod]
    [ExpectedException(typeof(RecompensaNoEncontradaException))]
    public void ActualizarRecompensa_NoExiste_LanzarError()
    {
        var recompensa = new Recompensa("TV", "descripcion", 100, 10, NivelMembresia.Estandar);
        _repoRecompensa.Setup(r => r.ObtenerPorId(recompensa.Id)).Returns(recompensa);
        RecompensaCreadoDto dto = _recompensaService.ObtenerRecompensa(recompensa.Id);
        dto.Id = Guid.NewGuid();
        var dtoCrear = new RecompensaCrearDto
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Costo = dto.Costo,
            CantidadDisponible = dto.CantidadDisponible,
            NivelRequerido = dto.NivelRequerido
        };
        _recompensaService.ActualizarRecompensa(dto.Id, dtoCrear);
    }

    [TestMethod]
    public void EliminarRecompensa_Existe_Eliminado()
    {
        var recompensa = new Recompensa("TV", "descripcion", 100, 10, NivelMembresia.Estandar);
        _repoRecompensa.Setup(r => r.ObtenerPorId(recompensa.Id)).Returns(recompensa);
        RecompensaCreadoDto dto = _recompensaService.ObtenerRecompensa(recompensa.Id);
        _recompensaService.EliminarRecompensa(dto.Id);
        _repoRecompensa.Setup(r => r.ObtenerTodos()).Returns([]);
        Assert.IsNull(_recompensaService.ObtenerTodos().FirstOrDefault());
    }

    [TestMethod]
    [ExpectedException(typeof(RecompensaNoEncontradaException))]
    public void EliminarRecompensa_NoExiste_Error()
    {
        _recompensaService.EliminarRecompensa(Guid.NewGuid());
    }
}
