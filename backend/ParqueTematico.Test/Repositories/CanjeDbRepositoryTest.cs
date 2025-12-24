using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;

namespace Test.Repositories;

[TestClass]
public class CanjeDbRepositoryTest
{
    private ParqueDbContext _context = null!;
    private CanjeDbRepository _repository = null!;
    private UsuarioDbRepository _usuarioRepo = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _repository = new CanjeDbRepository(_context);
        _usuarioRepo = new UsuarioDbRepository(_context);
    }

    private static Visitante CrearVisitanteValido()
    {
        return new Visitante(
            "Juan",
            "Perez",
            "juan.perez@test.com",
            "ClaveSegura1!",
            new DateTime(1990, 3, 15),
            NivelMembresia.Estandar);
    }

    private static Recompensa CrearRecompensaValida()
    {
        return new Recompensa(
            "Descuento 20%",
            "Descuento del 20% en tu próxima visita",
            100,
            50,
            NivelMembresia.Estandar);
    }

    [TestMethod]
    public void Agregar_CanjeValido_GuardaCorrectamente()
    {
        Visitante visitante = CrearVisitanteValido();
        _usuarioRepo.Agregar(visitante);
        Recompensa recompensa = CrearRecompensaValida();
        _context.Recompensas.Add(recompensa);
        _context.SaveChanges();

        var canje = new Canje(visitante, recompensa, DateTime.Now);

        Canje creado = _repository.Agregar(canje);
        _context.SaveChanges();

        Assert.IsTrue(creado.Id != Guid.Empty);
        Canje? enDb = _context.Canjes.FirstOrDefault(x => x.Id == creado.Id);
        Assert.IsNotNull(enDb);
        Assert.AreEqual(visitante.Id, enDb.Usuario.Id);
        Assert.AreEqual(recompensa.Id, enDb.Recompensa.Id);
    }

    [TestMethod]
    public void Agregar_IncrementaCantidad()
    {
        Visitante visitante = CrearVisitanteValido();
        _usuarioRepo.Agregar(visitante);
        Recompensa recompensa = CrearRecompensaValida();
        _context.Recompensas.Add(recompensa);
        _context.SaveChanges();

        var antes = _context.Canjes.Count();
        var canje = new Canje(visitante, recompensa, DateTime.Now);

        _repository.Agregar(canje);
        _context.SaveChanges();

        var despues = _context.Canjes.Count();
        Assert.AreEqual(antes + 1, despues);
    }

    [TestMethod]
    public void ObtenerPorId_Existente_DevuelveCanje()
    {
        Visitante visitante = CrearVisitanteValido();
        _usuarioRepo.Agregar(visitante);
        Recompensa recompensa = CrearRecompensaValida();
        _context.Recompensas.Add(recompensa);
        var canje = new Canje(visitante, recompensa, DateTime.Now);
        _repository.Agregar(canje);
        _context.SaveChanges();

        Canje? resultado = _repository.ObtenerPorId(canje.Id);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(canje.Id, resultado!.Id);
    }

    [TestMethod]
    public void ObtenerPorId_Inexistente_DevuelveNull()
    {
        Canje? resultado = _repository.ObtenerPorId(Guid.NewGuid());
        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void Eliminar_Existente_QuitaCanje()
    {
        Visitante visitante = CrearVisitanteValido();
        _usuarioRepo.Agregar(visitante);
        Recompensa recompensa = CrearRecompensaValida();
        _context.Recompensas.Add(recompensa);
        var canje = new Canje(visitante, recompensa, DateTime.Now);
        _repository.Agregar(canje);
        _context.SaveChanges();

        _repository.Eliminar(canje.Id);

        Canje? enDb = _context.Canjes.Find(canje.Id);
        Assert.IsNull(enDb);
    }

    [TestMethod]
    public void Eliminar_DisminuyeCantidad()
    {
        Visitante visitante = CrearVisitanteValido();
        _usuarioRepo.Agregar(visitante);
        Recompensa recompensa = CrearRecompensaValida();
        _context.Recompensas.Add(recompensa);
        var canje = new Canje(visitante, recompensa, DateTime.Now);
        _repository.Agregar(canje);
        _context.SaveChanges();

        var antes = _context.Canjes.Count();
        _repository.Eliminar(canje.Id);
        var despues = _context.Canjes.Count();

        Assert.AreEqual(antes - 1, despues);
    }

    [TestMethod]
    public void ObtenerTodos_DevuelveListaConCanjesExistentes()
    {
        Visitante visitante = CrearVisitanteValido();
        _usuarioRepo.Agregar(visitante);
        Recompensa r1 = CrearRecompensaValida();
        var r2 = new Recompensa("Entrada Gratis", "Una entrada gratis", 200, 10);
        _context.Recompensas.Add(r1);
        _context.Recompensas.Add(r2);
        _context.SaveChanges();

        var c1 = new Canje(visitante, r1, DateTime.Now);
        var c2 = new Canje(visitante, r2, DateTime.Now.AddDays(-1));
        _repository.Agregar(c1);
        _repository.Agregar(c2);
        _context.SaveChanges();

        IList<Canje> lista = _repository.ObtenerTodos();

        Assert.AreEqual(2, lista.Count);
        Assert.IsTrue(lista.Any(x => x.Id == c1.Id));
        Assert.IsTrue(lista.Any(x => x.Id == c2.Id));
    }

    [TestMethod]
    public void ObtenerTodos_SinCanjes_DevuelveListaVacia()
    {
        IList<Canje> lista = _repository.ObtenerTodos();
        Assert.AreEqual(0, lista.Count);
    }

    [TestMethod]
    public void ObtenerPorVisitante_DevuelveCanjesDelVisitante()
    {
        Visitante v1 = CrearVisitanteValido();
        var v2 = new Visitante("Maria", "Lopez", "maria@test.com", "ClaveSegura1!", new DateTime(1992, 5, 20), NivelMembresia.Premium);
        _usuarioRepo.Agregar(v1);
        _usuarioRepo.Agregar(v2);

        Recompensa r1 = CrearRecompensaValida();
        var r2 = new Recompensa("Regalo", "Un regalo especial", 150, 20);
        _context.Recompensas.Add(r1);
        _context.Recompensas.Add(r2);
        _context.SaveChanges();

        var c1 = new Canje(v1, r1, DateTime.Now);
        var c2 = new Canje(v1, r2, DateTime.Now.AddDays(-2));
        var c3 = new Canje(v2, r1, DateTime.Now);
        _repository.Agregar(c1);
        _repository.Agregar(c2);
        _repository.Agregar(c3);
        _context.SaveChanges();

        IList<Canje> canjesV1 = _repository.ObtenerPorVisitante(v1.Id);

        Assert.AreEqual(2, canjesV1.Count);
        Assert.IsTrue(canjesV1.All(c => c.Usuario.Id == v1.Id));
        Assert.IsTrue(canjesV1.Any(c => c.Id == c1.Id));
        Assert.IsTrue(canjesV1.Any(c => c.Id == c2.Id));
    }

    [TestMethod]
    public void ObtenerPorVisitante_VisitanteSinCanjes_DevuelveListaVacia()
    {
        Visitante visitante = CrearVisitanteValido();
        _usuarioRepo.Agregar(visitante);
        _context.SaveChanges();

        IList<Canje> canjes = _repository.ObtenerPorVisitante(visitante.Id);

        Assert.AreEqual(0, canjes.Count);
    }

    [TestMethod]
    public void ObtenerPorVisitante_OrdenadoPorFechaDescendente()
    {
        Visitante visitante = CrearVisitanteValido();
        _usuarioRepo.Agregar(visitante);
        Recompensa r1 = CrearRecompensaValida();
        var r2 = new Recompensa("Otro", "Descripcion", 50, 10);
        _context.Recompensas.Add(r1);
        _context.Recompensas.Add(r2);
        _context.SaveChanges();

        var c1 = new Canje(visitante, r1, DateTime.Now.AddDays(-5));
        var c2 = new Canje(visitante, r2, DateTime.Now);
        _repository.Agregar(c1);
        _repository.Agregar(c2);
        _context.SaveChanges();

        IList<Canje> canjes = _repository.ObtenerPorVisitante(visitante.Id);

        Assert.AreEqual(2, canjes.Count);
        Assert.AreEqual(c2.Id, canjes[0].Id);
        Assert.AreEqual(c1.Id, canjes[1].Id);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }
}
