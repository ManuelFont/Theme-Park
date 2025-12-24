using Dominio.Entities;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;

namespace Test.Repositories;

[TestClass]
public class BaseRepositoryTest
{
    private ParqueDbContext _context = null!;
    private BaseRepository<Recompensa> _repo = null!;

    [TestInitialize]
    public void SetUp()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _repo = new BaseRepository<Recompensa>(_context);
    }

    [TestMethod]
    public void Add_RecompensaGuardadaEnContext()
    {
        var recompensa = new Recompensa("Tv", "Des", 100, 10);
        _repo.Agregar(recompensa);
        Assert.AreEqual(recompensa.Id, _context.Recompensas.FirstOrDefault()!.Id);
    }

    [TestMethod]
    public void GetAll_SeRetornaListaConTodosLosElementos()
    {
        var recompensa = new Recompensa("Tv", "Des", 100, 10);
        _repo.Agregar(recompensa);
        var recompensa2 = new Recompensa("Viaje", "lindo viaje", 500, 1);
        _repo.Agregar(recompensa2);
        IList<Recompensa> listaRecompensas = _repo.ObtenerTodos();
        Assert.AreEqual(recompensa.Id, listaRecompensas[0].Id);
        Assert.AreEqual(recompensa2.Id, listaRecompensas[1].Id);
    }

    [TestMethod]
    public void GetById_IdExiste_RetornaRecompensaCorrecta()
    {
        var recompensa = new Recompensa("Tv", "Des", 100, 10);
        _repo.Agregar(recompensa);
        Recompensa? recompensaPorId = _repo.ObtenerPorId(recompensa.Id);
        Assert.IsNotNull(recompensaPorId);
        Assert.AreEqual("Tv", recompensaPorId.Nombre);
    }

    [TestMethod]
    public void GetById_IdNoExiste_RetornaNull()
    {
        var recompensa = new Recompensa("Tv", "Des", 100, 10);
        _repo.Agregar(recompensa);
        Recompensa? recompensaPorId = _repo.ObtenerPorId(Guid.NewGuid());
        Assert.IsNull(recompensaPorId);
    }

    [TestMethod]
    public void Update_RecompensaExiste_SeActualizaCorrectamente()
    {
        var recompensa = new Recompensa("Tv", "Des", 100, 10);
        _repo.Agregar(recompensa);
        recompensa.Nombre = "Tv 40 pulgadas";
        _repo.Actualizar(recompensa);
        Assert.AreEqual("Tv 40 pulgadas", _context.Recompensas.FirstOrDefault()!.Nombre);
    }

    [TestMethod]
    public void Delete_RecompensaExiste_Eliminar()
    {
        var recompensa = new Recompensa("Tv", "Des", 100, 10);
        _repo.Agregar(recompensa);
        _repo.Eliminar(recompensa.Id);
        Assert.IsNull(_context.Recompensas.FirstOrDefault()!);
    }
}
