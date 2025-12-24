using Dominio.Entities;
using Dtos;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using ParqueTematico.Application.Services;
using RepositoryInterfaces;

namespace Test.Application;

[TestClass]
public class AtraccionServiceTests
{
    private ParqueDbContext _ctx = null!;
    private IAtraccionRepository _repo = null!;
    private AtraccionService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _ctx = SqlContextFactory.CreateMemoryContext();
        _repo = new AtraccionRepository(_ctx);
        _service = new AtraccionService(_repo);
    }

    [TestMethod]
    public void Crear_Y_ObtenerPorId_DeberiaFuncionar()
    {
        _service.Crear("Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Divertida", true);

        Atraccion? atraccion = _repo.ObtenerTodos().FirstOrDefault();
        Assert.IsNotNull(atraccion);
        Assert.AreEqual("Rusa", atraccion.Nombre);
    }

    [TestMethod]
    public void Actualizar_TodosCampos_DeberiaCambiarValores()
    {
        _service.Crear("Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Divertida", true);
        Atraccion atraccion = _repo.ObtenerTodos().First();
        _ = AtraccionDto.FromEntity(atraccion);
        var nuevoDto = new AtraccionDto
        {
            Id = atraccion.Id,
            Nombre = "Nueva Rusa",
            Tipo = TipoAtraccion.Simulador,
            EdadMinima = 15,
            CapacidadMaxima = 25,
            Descripcion = "Mas emocionante"
        };

        _service.Actualizar(nuevoDto);

        Atraccion? actualizado = _repo.ObtenerPorId(atraccion.Id);
        Assert.AreEqual("Nueva Rusa", actualizado!.Nombre);
        Assert.AreEqual(TipoAtraccion.Simulador, actualizado.Tipo);
        Assert.AreEqual(15, actualizado.EdadMinima);
        Assert.AreEqual(25, actualizado.CapacidadMaxima);
        Assert.AreEqual("Mas emocionante", actualizado.Descripcion);
    }

    [TestMethod]
    public void ObtenerPorId_CuandoExiste_DeberiaRetornarAtraccion()
    {
        _service.Crear("Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Divertida", true);
        Guid id = _repo.ObtenerTodos().First().Id;

        Atraccion? atraccion = _service.ObtenerPorId(id);

        Assert.IsNotNull(atraccion);
        Assert.AreEqual("Rusa", atraccion.Nombre);
        Assert.AreEqual(id, atraccion.Id);
    }

    [TestMethod]
    public void ObtenerPorId_CuandoNoExiste_DeberiaRetornarNull()
    {
        Atraccion? atraccion = _service.ObtenerPorId(Guid.NewGuid());

        Assert.IsNull(atraccion);
    }

    [TestMethod]
    public void ObtenerTodos_DeberiaRetornarTodasLasAtracciones()
    {
        _service.Crear("Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Divertida", true);
        _service.Crear("Simulador", TipoAtraccion.Simulador, 10, 15, "Virtual", true);
        _service.Crear("Show", TipoAtraccion.Espectáculo, 0, 100, "En vivo", true);

        IEnumerable<Atraccion> atracciones = _service.ObtenerTodos();

        Assert.AreEqual(3, atracciones.Count());
        Assert.IsTrue(atracciones.Any(a => a.Nombre == "Rusa"));
        Assert.IsTrue(atracciones.Any(a => a.Nombre == "Simulador"));
        Assert.IsTrue(atracciones.Any(a => a.Nombre == "Show"));
    }

    [TestMethod]
    public void ObtenerTodos_CuandoNoHayAtracciones_DeberiaRetornarVacio()
    {
        IEnumerable<Atraccion> atracciones = _service.ObtenerTodos();

        Assert.AreEqual(0, atracciones.Count());
    }
}
