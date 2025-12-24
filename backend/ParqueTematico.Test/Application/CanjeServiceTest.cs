using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Dtos;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Moq;
using ParqueTematico.Application.Services;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace Test.Application;

[TestClass]
public class CanjeServiceTest
{
    private Mock<ICanjeRepository> _canjeRepo = null!;
    private ICanjeService _canjeService = null!;
    private ParqueDbContext _context = null!;
    private Mock<IBaseRepository<Recompensa>> _recompensaRepo = null!;
    private RelojService _reloj = null!;
    private IRelojRepository _relojRepo = null!;
    private Mock<IUsuarioRepository> _usuarioRepo = null!;

    [TestInitialize]
    public void StartUp()
    {
        _canjeRepo = new Mock<ICanjeRepository>();
        _usuarioRepo = new Mock<IUsuarioRepository>();
        _recompensaRepo = new Mock<IBaseRepository<Recompensa>>();
        _context = SqlContextFactory.CreateMemoryContext();
        _relojRepo = new RelojDbRepository(_context);
        _reloj = new RelojService(_relojRepo);
        _canjeService = new CanjeService(_canjeRepo.Object, _usuarioRepo.Object, _recompensaRepo.Object, _reloj);
    }

    [TestMethod]
    public void CrearCanje_DatosCorrectos_DatosCorrectosCantidadRestada()
    {
        var visitante = new Visitante("Manuel", "Font", "manu@mail.com", "Manu1234!", new DateTime(2004, 7, 21),
            NivelMembresia.Estandar);
        visitante.AgregarPuntos(100);
        var recompensa = new Recompensa("Tv", "40 pulgadas", 20, 20, NivelMembresia.Estandar);
        var canjeCrearDto = new CanjeCrearDto { UsuarioId = visitante.Id, RecompensaId = recompensa.Id };
        _usuarioRepo.Setup(r => r.ObtenerPorId(visitante.Id)).Returns(visitante);
        _recompensaRepo.Setup(r => r.ObtenerPorId(recompensa.Id)).Returns(recompensa);
        CanjeCreadoDto canjeCreadoDto = _canjeService.CrearCanje(canjeCrearDto);

        Assert.AreEqual(visitante.Id, canjeCreadoDto.UsuarioId);
        Assert.AreEqual(recompensa.Id, canjeCreadoDto.RecompensaId);
        Assert.AreEqual(_reloj.ObtenerFechaHora(), canjeCreadoDto.FechaCanje);
        Assert.AreEqual(19, recompensa.CantidadDisponible);
        Assert.AreEqual(80, visitante.PuntosActuales);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CrearCanje_UsuarioNoExiste_LanzarError()
    {
        var visitante = new Visitante("Manuel", "Font", "manu@mail.com", "Manu1234!", new DateTime(2004, 7, 21),
            NivelMembresia.Estandar);
        var recompensa = new Recompensa("Tv", "40 pulgadas", 20, 20, NivelMembresia.Estandar);
        var canjeCrearDto = new CanjeCrearDto { UsuarioId = visitante.Id, RecompensaId = recompensa.Id };
        visitante = null;
        _usuarioRepo.Setup(r => r.ObtenerPorId(canjeCrearDto.UsuarioId)).Returns(visitante);
        _recompensaRepo.Setup(r => r.ObtenerPorId(recompensa.Id)).Returns(recompensa);
        _canjeService.CrearCanje(canjeCrearDto);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CrearCanje_UsuarioNoEsVisitante_LanzarError()
    {
        var operador = new Operador("Manuel", "Font", "manu@mail.com", "Manu1234!");
        var recompensa = new Recompensa("Tv", "40 pulgadas", 20, 20, NivelMembresia.Estandar);
        var canjeCrearDto = new CanjeCrearDto { UsuarioId = operador.Id, RecompensaId = recompensa.Id };
        _usuarioRepo.Setup(r => r.ObtenerPorId(canjeCrearDto.UsuarioId)).Returns(operador);
        _recompensaRepo.Setup(r => r.ObtenerPorId(recompensa.Id)).Returns(recompensa);
        _canjeService.CrearCanje(canjeCrearDto);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CrearCanje_RecompensaNoExiste_LanzarError()
    {
        var visitante = new Visitante("Manuel", "Font", "manu@mail.com", "Manu1234!", new DateTime(2004, 7, 21),
            NivelMembresia.Estandar);
        var recompensa = new Recompensa("Tv", "40 pulgadas", 20, 20, NivelMembresia.Estandar);
        var canjeCrearDto = new CanjeCrearDto { UsuarioId = visitante.Id, RecompensaId = recompensa.Id };
        recompensa = null;
        _usuarioRepo.Setup(r => r.ObtenerPorId(canjeCrearDto.UsuarioId)).Returns(visitante);
        _recompensaRepo.Setup(r => r.ObtenerPorId(canjeCrearDto.RecompensaId)).Returns(recompensa);
        _canjeService.CrearCanje(canjeCrearDto);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CrearCanje_CantidadInsuficiente_LanzarError()
    {
        var visitante = new Visitante("Manuel", "Font", "manu@mail.com", "Manu1234!", new DateTime(2004, 7, 21),
            NivelMembresia.Estandar);
        var recompensa = new Recompensa("Tv", "40 pulgadas", 20, 0, NivelMembresia.Estandar);
        var canjeCrearDto = new CanjeCrearDto { UsuarioId = visitante.Id, RecompensaId = recompensa.Id };
        _usuarioRepo.Setup(r => r.ObtenerPorId(canjeCrearDto.UsuarioId)).Returns(visitante);
        _recompensaRepo.Setup(r => r.ObtenerPorId(canjeCrearDto.RecompensaId)).Returns(recompensa);
        _canjeService.CrearCanje(canjeCrearDto);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CrearCanje_MembresiaInadecuada_LanzarError()
    {
        var visitante = new Visitante("Manuel", "Font", "manu@mail.com", "Manu1234!", new DateTime(2004, 7, 21),
            NivelMembresia.Estandar);
        var recompensa = new Recompensa("Tv", "40 pulgadas", 20, 0, NivelMembresia.Premium);
        var canjeCrearDto = new CanjeCrearDto { UsuarioId = visitante.Id, RecompensaId = recompensa.Id };
        _usuarioRepo.Setup(r => r.ObtenerPorId(canjeCrearDto.UsuarioId)).Returns(visitante);
        _recompensaRepo.Setup(r => r.ObtenerPorId(canjeCrearDto.RecompensaId)).Returns(recompensa);
        _canjeService.CrearCanje(canjeCrearDto);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CrearCanje_PuntosInsuficientes_LanzarError()
    {
        var visitante = new Visitante("Manuel", "Font", "manu@mail.com", "Manu1234!", new DateTime(2004, 7, 21),
            NivelMembresia.Estandar);
        visitante.AgregarPuntos(10);
        var recompensa = new Recompensa("Tv", "40 pulgadas", 20, 5, NivelMembresia.Estandar);
        var canjeCrearDto = new CanjeCrearDto { UsuarioId = visitante.Id, RecompensaId = recompensa.Id };
        _usuarioRepo.Setup(r => r.ObtenerPorId(canjeCrearDto.UsuarioId)).Returns(visitante);
        _recompensaRepo.Setup(r => r.ObtenerPorId(canjeCrearDto.RecompensaId)).Returns(recompensa);
        _canjeService.CrearCanje(canjeCrearDto);
    }

    [TestMethod]
    public void ObtenerTodos_RetornaListaCorrecta()
    {
        var visitante = new Visitante("Manuel", "Font", "manu@mail.com", "Manu1234!", new DateTime(2004, 7, 21),
            NivelMembresia.Estandar);
        var recompensa = new Recompensa("Tv", "40 pulgadas", 20, 20, NivelMembresia.Estandar);
        var canje = new Canje(visitante, recompensa, _reloj.ObtenerFechaHora());
        _canjeRepo.Setup(r => r.ObtenerTodos()).Returns([canje]);

        Assert.IsNotNull(_canjeService.ObtenerTodos().FirstOrDefault());
        Assert.AreEqual(canje.Id, _canjeService.ObtenerTodos().FirstOrDefault()!.Id);
        Assert.AreEqual(visitante.Id, _canjeService.ObtenerTodos().FirstOrDefault()!.UsuarioId);
        Assert.AreEqual(recompensa.Id, _canjeService.ObtenerTodos().FirstOrDefault()!.RecompensaId);
    }

    [TestMethod]
    public void ObtenerCanje_RetornaCanjeAdecuado()
    {
        var visitante = new Visitante("Manuel", "Font", "manu@mail.com", "Manu1234!", new DateTime(2004, 7, 21),
            NivelMembresia.Estandar);
        var recompensa = new Recompensa("Tv", "40 pulgadas", 20, 20, NivelMembresia.Estandar);
        var canje = new Canje(visitante, recompensa, _reloj.ObtenerFechaHora());
        _canjeRepo.Setup(r => r.ObtenerPorId(canje.Id)).Returns(canje);

        Assert.IsNotNull(_canjeService.ObtenerCanje(canje.Id));
        Assert.AreEqual(canje.Id, _canjeService.ObtenerCanje(canje.Id).Id);
        Assert.AreEqual(visitante.Id, _canjeService.ObtenerCanje(canje.Id).UsuarioId);
        Assert.AreEqual(recompensa.Id, _canjeService.ObtenerCanje(canje.Id).RecompensaId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ObtenerCanje_LanzaErrorSiNoExiste()
    {
        _canjeService.ObtenerCanje(Guid.NewGuid());
    }

    [TestMethod]
    public void Actualizar_ValoresValidos_ActualizaCorrectamente()
    {
        var visitante = new Visitante("Manuel", "Font", "manu@mail.com", "Manu1234!", new DateTime(2004, 7, 21),
            NivelMembresia.Estandar);
        visitante.AgregarPuntos(100);
        var recompensa = new Recompensa("Tv", "40 pulgadas", 20, 20, NivelMembresia.Estandar);
        var canje = new Canje(visitante, recompensa, _reloj.ObtenerFechaHora());

        var canjeCrearDto = new CanjeCrearDto { UsuarioId = visitante.Id, RecompensaId = recompensa.Id };
        _usuarioRepo.Setup(r => r.ObtenerPorId(visitante.Id)).Returns(visitante);
        _recompensaRepo.Setup(r => r.ObtenerPorId(recompensa.Id)).Returns(recompensa);
        CanjeCreadoDto canjeCreadoDto = _canjeService.CrearCanje(canjeCrearDto);
        canjeCreadoDto.Id = canje.Id;

        var visitante2 = new Visitante("Mateo", "Font", "mate@mail.com", "Mate1234!", new DateTime(2007, 7, 21),
            NivelMembresia.Estandar);
        canjeCrearDto.UsuarioId = visitante2.Id;
        _usuarioRepo.Setup(r => r.ObtenerPorId(visitante2.Id)).Returns(visitante2);

        _canjeRepo.Setup(r => r.ObtenerPorId(canjeCreadoDto.Id)).Returns(canje);
        _canjeService.ActualizarCanje(canjeCreadoDto.Id, canjeCrearDto);

        Assert.AreEqual(visitante2.Id, _canjeService.ObtenerCanje(canjeCreadoDto.Id).UsuarioId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Actualizar_CanjeNoExiste_LanzaError()
    {
        var visitante = new Visitante("Manuel", "Font", "manu@mail.com", "Manu1234!", new DateTime(2004, 7, 21),
            NivelMembresia.Estandar);
        var recompensa = new Recompensa("Tv", "40 pulgadas", 20, 20, NivelMembresia.Estandar);

        var canjeCrearDto = new CanjeCrearDto { UsuarioId = visitante.Id, RecompensaId = recompensa.Id };
        _usuarioRepo.Setup(r => r.ObtenerPorId(visitante.Id)).Returns(visitante);
        _recompensaRepo.Setup(r => r.ObtenerPorId(recompensa.Id)).Returns(recompensa);
        CanjeCreadoDto canjeCreadoDto = _canjeService.CrearCanje(canjeCrearDto);
        canjeCreadoDto.Id = Guid.NewGuid();
        _canjeService.ActualizarCanje(canjeCreadoDto.Id, canjeCrearDto);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Actualizar_RecompensaNoExiste_LanzaError()
    {
        var visitante = new Visitante("Manuel", "Font", "manu@mail.com", "Manu1234!", new DateTime(2004, 7, 21),
            NivelMembresia.Estandar);
        var recompensa = new Recompensa("Tv", "40 pulgadas", 20, 20, NivelMembresia.Estandar);

        var canjeCrearDto = new CanjeCrearDto { UsuarioId = visitante.Id, RecompensaId = recompensa.Id };
        _usuarioRepo.Setup(r => r.ObtenerPorId(visitante.Id)).Returns(visitante);
        _recompensaRepo.Setup(r => r.ObtenerPorId(recompensa.Id)).Returns(recompensa);
        CanjeCreadoDto canjeCreadoDto = _canjeService.CrearCanje(canjeCrearDto);
        canjeCreadoDto.RecompensaId = Guid.NewGuid();
        _canjeService.ActualizarCanje(canjeCreadoDto.Id, canjeCrearDto);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Actualizar_UsuarioNoExiste_LanzaError()
    {
        var visitante = new Visitante("Manuel", "Font", "manu@mail.com", "Manu1234!", new DateTime(2004, 7, 21),
            NivelMembresia.Estandar);
        var recompensa = new Recompensa("Tv", "40 pulgadas", 20, 20, NivelMembresia.Estandar);

        var canjeCrearDto = new CanjeCrearDto { UsuarioId = visitante.Id, RecompensaId = recompensa.Id };
        _usuarioRepo.Setup(r => r.ObtenerPorId(visitante.Id)).Returns(visitante);
        _recompensaRepo.Setup(r => r.ObtenerPorId(recompensa.Id)).Returns(recompensa);
        CanjeCreadoDto canjeCreadoDto = _canjeService.CrearCanje(canjeCrearDto);
        canjeCreadoDto.UsuarioId = Guid.NewGuid();
        _canjeService.ActualizarCanje(canjeCreadoDto.Id, canjeCrearDto);
    }

    [TestMethod]
    public void EliminarCanje_ValoresValidos_CanjeEliminado()
    {
        var visitante = new Visitante("Manuel", "Font", "manu@mail.com", "Manu1234!", new DateTime(2004, 7, 21),
            NivelMembresia.Estandar);
        visitante.AgregarPuntos(100);
        var recompensa = new Recompensa("Tv", "40 pulgadas", 20, 20, NivelMembresia.Estandar);
        var canje = new Canje(visitante, recompensa, _reloj.ObtenerFechaHora());
        var canjeCrearDto = new CanjeCrearDto { UsuarioId = visitante.Id, RecompensaId = recompensa.Id };
        _usuarioRepo.Setup(r => r.ObtenerPorId(visitante.Id)).Returns(visitante);
        _recompensaRepo.Setup(r => r.ObtenerPorId(recompensa.Id)).Returns(recompensa);
        CanjeCreadoDto canjeCreadoDto = _canjeService.CrearCanje(canjeCrearDto);
        canjeCreadoDto.Id = canje.Id;
        _canjeRepo.Setup(r => r.ObtenerPorId(canje.Id)).Returns(canje);
        _canjeService.EliminarCanje(canjeCreadoDto.Id);
        _canjeRepo.Setup(r => r.ObtenerTodos()).Returns([]);
        Assert.IsNull(_canjeService.ObtenerTodos().FirstOrDefault());
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void EliminarCanje_NoExisteCanje_LanzarError()
    {
        _canjeService.EliminarCanje(Guid.NewGuid());
    }
}
