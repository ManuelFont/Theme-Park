using Dominio.Entities;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;

namespace Test.Repositories;

[TestClass]
public class RelojDbRepositoryTest
{
    private ParqueDbContext _context = null!;
    private RelojDbRepository _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _repository = new RelojDbRepository(_context);
    }

    [TestMethod]
    public void Agregar_AgregaRelojCorrectamente()
    {
        var reloj = new Reloj();
        _repository.Agregar(reloj);
        Assert.AreEqual(reloj.Id, _context.Relojes.First().Id);
    }

    [TestMethod]
    public void Actualizar_ActualizaDatosReloj()
    {
        var reloj = new Reloj();
        _repository.Agregar(reloj);
        reloj.FechaHora = DateTime.Today.AddDays(1);
        _repository.Actualizar(reloj);
        Reloj? relojDb = _context.Relojes.Find(reloj.Id);
        Assert.IsNotNull(relojDb);
        Assert.AreEqual(reloj.FechaHora, relojDb!.FechaHora);
    }

    [TestMethod]
    public void ObtenerPorId_DevuelveRelojCorrecto()
    {
        var reloj = new Reloj();
        _repository.Agregar(reloj);
        Assert.IsNotNull(_repository.ObtenerPorId(1));
        Assert.AreEqual(reloj.FechaHora, _repository.ObtenerPorId(1)!.FechaHora);
    }
}
