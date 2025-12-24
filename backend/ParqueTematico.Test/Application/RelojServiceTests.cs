using Dominio.Entities;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using ParqueTematico.Application.Services;

namespace Test.Application;

[TestClass]
public class RelojServiceTests
{
    private ParqueDbContext _context = null!;
    private RelojDbRepository _repository = null!;
    private RelojService _service = null!;

    [TestInitialize]
    public void SetUp()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _repository = new RelojDbRepository(_context);
        _service = new RelojService(_repository);
    }

    [TestMethod]
    [ExpectedException(typeof(NullReferenceException))]
    public void ModificarFechaHora_RelojNoExiste_Error()
    {
        _repository.Eliminar(1);
        _service.ModificarFechaHora(DateTime.Now);
    }

    [TestMethod]
    public void ModificarFechaHora_SeModificaCorrectamente()
    {
        DateTime fechaHora = DateTime.Now;
        _service.ModificarFechaHora(fechaHora);
        Reloj? reloj = _repository.ObtenerPorId(1);
        Assert.IsNotNull(reloj);
        Assert.AreEqual(fechaHora, reloj.FechaHora);
    }

    [TestMethod]
    [ExpectedException(typeof(NullReferenceException))]
    public void ObtenerFechaHora_RelojNoExiste_Error()
    {
        _repository.Eliminar(1);
        _service.ObtenerFechaHora();
    }

    [TestMethod]
    public void ObtenerFechaHora_RetornaCorrectamente()
    {
        DateTime fechaHora = DateTime.Now;
        _service.ModificarFechaHora(fechaHora);
        Reloj? reloj = _repository.ObtenerPorId(1);
        Assert.IsNotNull(reloj);
        Assert.AreEqual(reloj.FechaHora, _service.ObtenerFechaHora());
    }
}
