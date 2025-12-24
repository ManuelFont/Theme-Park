using System.Security.Claims;
using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Dtos;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ParqueTematico.Application.Services;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Controllers;

namespace Test.WebApi;

[TestClass]
public class HistorialPuntuacionControllerTests
{
    private ParqueDbContext _context = null!;
    private HistorialPuntuacionController _controller = null!;
    private HistorialPuntuacionDbRepository _historialRepo = null!;
    private HistorialPuntuacionService _serviceService = null!;
    private UsuarioDbRepository _usuarioRepo = null!;
    private Visitante _visitante = null!;
    private Mock<IAuthService> _mockAuthService = null!;

    [TestInitialize]
    public void SetUp()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _historialRepo = new HistorialPuntuacionDbRepository(_context);
        _usuarioRepo = new UsuarioDbRepository(_context);
        _serviceService = new HistorialPuntuacionService(_historialRepo);
        _mockAuthService = new Mock<IAuthService>();
        _mockAuthService.Setup(a => a.ObtenerUserIdDeClaims(It.IsAny<ClaimsPrincipal>()))
            .Returns<ClaimsPrincipal>(user =>
            {
                var idClaim = user.FindFirst("id");
                if(idClaim == null)
                {
                    throw new InvalidOperationException("No se encontró el claim 'id'");
                }

                return Guid.Parse(idClaim.Value);
            });

        _controller = new HistorialPuntuacionController(_serviceService, _mockAuthService.Object);

        _visitante = new Visitante("Juan", "Pérez", "juan@test.com", "Pass123!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        _usuarioRepo.Agregar(_visitante);
    }

    [TestMethod]
    public void ObtenerHistorialPorVisitante_SinHistorial_DebeRetornarListaVacia()
    {
        ActionResult<List<HistorialPuntuacionDto>> resultado = _controller.ObtenerHistorialPorVisitante(_visitante.Id);
        var okResult = resultado.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        var historial = okResult.Value as List<HistorialPuntuacionDto>;
        Assert.IsNotNull(historial);
        Assert.AreEqual(0, historial.Count);
    }

    [TestMethod]
    public void ObtenerHistorialPorVisitante_ConRegistros_DebeRetornarHistorialOrdenado()
    {
        var fecha1 = new DateTime(2025, 10, 27, 10, 0, 0);
        var fecha2 = new DateTime(2025, 10, 27, 14, 0, 0);
        var fecha3 = new DateTime(2025, 10, 27, 12, 0, 0);

        var historial1 =
            new HistorialPuntuacion(_visitante, 50, "Acceso a Montaña Rusa", "PuntuacionPorAtraccion", fecha1);
        var historial2 = new HistorialPuntuacion(_visitante, 100, "Evento especial", "PuntuacionPorEvento", fecha2);
        var historial3 = new HistorialPuntuacion(_visitante, 75, "Canje de recompensa", "PuntuacionCombo", fecha3);

        _serviceService.RegistrarHistorial(historial1);
        _serviceService.RegistrarHistorial(historial2);
        _serviceService.RegistrarHistorial(historial3);

        ActionResult<List<HistorialPuntuacionDto>> resultado = _controller.ObtenerHistorialPorVisitante(_visitante.Id);
        var okResult = resultado.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        var historial = okResult.Value as List<HistorialPuntuacionDto>;
        Assert.IsNotNull(historial);
        Assert.AreEqual(3, historial.Count);

        Assert.AreEqual(fecha2, historial[0].FechaHora);
        Assert.AreEqual(100, historial[0].Puntos);
        Assert.AreEqual("Evento especial", historial[0].Origen);
        Assert.AreEqual("PuntuacionPorEvento", historial[0].EstrategiaActiva);

        Assert.AreEqual(fecha3, historial[1].FechaHora);
        Assert.AreEqual(75, historial[1].Puntos);

        Assert.AreEqual(fecha1, historial[2].FechaHora);
        Assert.AreEqual(50, historial[2].Puntos);
    }

    [TestMethod]
    public void ObtenerHistorialPorVisitante_VisitanteNoExiste_DebeRetornarListaVacia()
    {
        var idInexistente = Guid.NewGuid();

        ActionResult<List<HistorialPuntuacionDto>> resultado = _controller.ObtenerHistorialPorVisitante(idInexistente);
        var okResult = resultado.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        var historial = okResult.Value as List<HistorialPuntuacionDto>;
        Assert.IsNotNull(historial);
        Assert.AreEqual(0, historial.Count);
    }

    [TestMethod]
    public void ObtenerHistorialPorVisitante_ConPuntosNegativos_DebeIncluirlos()
    {
        var fecha = new DateTime(2025, 10, 27, 10, 0, 0);

        var historial1 =
            new HistorialPuntuacion(_visitante, 100, "Acceso a atracción", "PuntuacionPorAtraccion", fecha);
        var historial2 = new HistorialPuntuacion(_visitante, -50, "Canje de recompensa", "PuntuacionPorAtraccion",
            fecha.AddHours(1));

        _serviceService.RegistrarHistorial(historial1);
        _serviceService.RegistrarHistorial(historial2);

        ActionResult<List<HistorialPuntuacionDto>> resultado = _controller.ObtenerHistorialPorVisitante(_visitante.Id);
        var okResult = resultado.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        var historial = okResult.Value as List<HistorialPuntuacionDto>;
        Assert.IsNotNull(historial);
        Assert.AreEqual(2, historial.Count);

        Assert.AreEqual(-50, historial[0].Puntos);
        Assert.AreEqual(100, historial[1].Puntos);
    }

    [TestMethod]
    public void ObtenerMiHistorial_SinHistorial_DebeRetornarListaVacia()
    {
        var claims = new List<Claim> { new("id", _visitante.Id.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        ActionResult<List<HistorialPuntuacionDto>> resultado = _controller.ObtenerMiHistorial();
        var okResult = resultado.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        var historial = okResult.Value as List<HistorialPuntuacionDto>;
        Assert.IsNotNull(historial);
        Assert.AreEqual(0, historial.Count);
    }

    [TestMethod]
    public void ObtenerMiHistorial_ConRegistros_DebeRetornarHistorialOrdenado()
    {
        var fecha1 = new DateTime(2025, 10, 27, 10, 0, 0);
        var fecha2 = new DateTime(2025, 10, 27, 14, 0, 0);
        var fecha3 = new DateTime(2025, 10, 27, 12, 0, 0);

        var historial1 = new HistorialPuntuacion(_visitante, 50, "Acceso a Montaña Rusa", "PuntuacionPorAtraccion", fecha1);
        var historial2 = new HistorialPuntuacion(_visitante, 100, "Evento especial", "PuntuacionPorEvento", fecha2);
        var historial3 = new HistorialPuntuacion(_visitante, 75, "Canje de recompensa", "PuntuacionCombo", fecha3);

        _serviceService.RegistrarHistorial(historial1);
        _serviceService.RegistrarHistorial(historial2);
        _serviceService.RegistrarHistorial(historial3);

        var claims = new List<Claim> { new("id", _visitante.Id.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        ActionResult<List<HistorialPuntuacionDto>> resultado = _controller.ObtenerMiHistorial();
        var okResult = resultado.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        var historial = okResult.Value as List<HistorialPuntuacionDto>;
        Assert.IsNotNull(historial);
        Assert.AreEqual(3, historial.Count);

        Assert.AreEqual(fecha2, historial[0].FechaHora);
        Assert.AreEqual(100, historial[0].Puntos);
        Assert.AreEqual("Evento especial", historial[0].Origen);
        Assert.AreEqual("PuntuacionPorEvento", historial[0].EstrategiaActiva);

        Assert.AreEqual(fecha3, historial[1].FechaHora);
        Assert.AreEqual(75, historial[1].Puntos);

        Assert.AreEqual(fecha1, historial[2].FechaHora);
        Assert.AreEqual(50, historial[2].Puntos);
    }

    [TestMethod]
    public void ObtenerMiHistorial_ConPuntosNegativos_DebeIncluirlos()
    {
        var fecha = new DateTime(2025, 10, 27, 10, 0, 0);

        var historial1 = new HistorialPuntuacion(_visitante, 100, "Acceso a atracción", "PuntuacionPorAtraccion", fecha);
        var historial2 = new HistorialPuntuacion(_visitante, -50, "Canje de recompensa", "PuntuacionPorAtraccion", fecha.AddHours(1));

        _serviceService.RegistrarHistorial(historial1);
        _serviceService.RegistrarHistorial(historial2);

        var claims = new List<Claim> { new("id", _visitante.Id.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        ActionResult<List<HistorialPuntuacionDto>> resultado = _controller.ObtenerMiHistorial();
        var okResult = resultado.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        var historial = okResult.Value as List<HistorialPuntuacionDto>;
        Assert.IsNotNull(historial);
        Assert.AreEqual(2, historial.Count);

        Assert.AreEqual(-50, historial[0].Puntos);
        Assert.AreEqual(100, historial[1].Puntos);
    }
}
