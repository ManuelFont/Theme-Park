using Dominio.Entities.Usuarios;
using Dominio.Exceptions;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;

namespace Test.Repositories;

[TestClass]
public class UsuarioDbRepositoryTest
{
    private ParqueDbContext _context = null!;
    private UsuarioDbRepository _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _repository = new UsuarioDbRepository(_context);
    }

    [TestMethod]
    public void Add_Saves_User_And_Assigns_Id()
    {
        var u = new Operador("Ana", "Suarez", "Ana@Acme.com", "Aa123456!");

        Usuario saved = _repository.Agregar(u);

        Assert.IsNotNull(saved);
        Assert.AreNotEqual(Guid.Empty, saved.Id);
        Assert.AreEqual("ana@acme.com", saved.Email);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void Add_Throws_When_Email_Duplicated()
    {
        var admin = new Administrador("Ana", "Suarez", "dup@acme.com", "Aa123456!");
        var visitante = new Visitante("Juan", "Paz", "dup@acme.com", "Aa123456!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);

        _repository.Agregar(admin);
        _repository.Agregar(visitante);
    }

    [TestMethod]
    public void GetById_Returns_User_When_Exists()
    {
        var v = new Visitante("Pepe", "Latorre", "Pepe@gmail.com", "Aa123456!", new DateTime(2000, 1, 1),
            NivelMembresia.Premium);
        Usuario saved = _repository.Agregar(v);

        Usuario? found = _repository.ObtenerPorId(saved.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(saved.Id, found!.Id);
        Assert.AreEqual("pepe@gmail.com", found.Email);
    }

    [TestMethod]
    public void GetById_Returns_Null_When_Not_Found()
    {
        Usuario? found = _repository.ObtenerPorId(Guid.NewGuid());
        Assert.IsNull(found);
    }

    [TestMethod]
    public void GetAll_Returns_All_Inserted_Users()
    {
        _repository.Agregar(new Operador("Ana", "Suarez", "ana@gmail.com", "Aa123456!"));
        _repository.Agregar(new Administrador("Luis", "Paz", "luis@gmail.com", "Aa123456!"));

        IList<Usuario> all = _repository.ObtenerTodos();

        Assert.AreEqual(2, all.Count);
        Assert.IsTrue(all.Any(u => u.Email == "ana@gmail.com"));
        Assert.IsTrue(all.Any(u => u.Email == "luis@gmail.com"));
    }

    private static void AsignarIdPrivado(Usuario usuario, Guid id)
    {
        typeof(Usuario).GetProperty(nameof(Usuario.Id))!
            .SetValue(usuario, id);
    }

    [TestMethod]
    public void Update_DeberiaActualizarCamposBaseYDeVisitante()
    {
        var visitanteOriginal = new Visitante(
            "Luz", "Soto", "luz@gmail.com", "Aa123456!",
            new DateTime(2000, 1, 1), NivelMembresia.Estandar);
        Usuario visitanteGuardado = _repository.Agregar(visitanteOriginal);
        var visitanteModificado = new Visitante(
            "Luz", "Soto", "luz2@gmail.com", "Bb987654!",
            new DateTime(1999, 12, 31), NivelMembresia.Premium);

        AsignarIdPrivado(visitanteModificado, visitanteGuardado.Id);
        _repository.Actualizar(visitanteModificado);

        Usuario? resultado = _repository.ObtenerPorId(visitanteGuardado.Id);
        Assert.IsNotNull(resultado);
        Assert.AreEqual("luz2@gmail.com", resultado!.Email);
        Assert.IsTrue(resultado.ValidarContrasenia("Bb987654!"));

        var visitanteResultado = (Visitante)resultado;
        Assert.AreEqual(new DateTime(1999, 12, 31), visitanteResultado.FechaNacimiento);
        Assert.AreEqual(NivelMembresia.Premium, visitanteResultado.NivelMembresia);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void Update_DeberiaLanzarCuandoElEmailYaExisteEnOtroUsuario()
    {
        _ = _repository.Agregar(
            new Administrador("Ana", "Suarez", "ana@gmail.com", "Aa123456!"));

        Usuario operadorAActualizar = _repository.Agregar(
            new Operador("Luis", "Paz", "luis@gmail.com", "Aa123456!"));

        var operadorConEmailDuplicado = new Operador(
            "Luis", "Paz", "ana@gmail.com", "Aa123456!");
        AsignarIdPrivado(operadorConEmailDuplicado, operadorAActualizar.Id);

        _repository.Actualizar(operadorConEmailDuplicado);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void Update_DeberiaLanzarCuandoElUsuarioNoExiste()
    {
        var inexistente = new Operador("nombr", "apellido", "inextistente@gmail.com", "Aa123456!");
        _repository.Actualizar(inexistente);
    }

    [TestMethod]
    public void Delete_DeberiaEliminarElUsuario()
    {
        Usuario creado = _repository.Agregar(new Operador("Ana", "Suarez", "ana@gmail.com", "Aa123456!"));
        Guid id = creado.Id;
        _repository.Eliminar(id);
        Usuario? buscado = _repository.ObtenerPorId(id);
        Assert.IsNull(buscado);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void Delete_DeberiaLanzarSiNoExiste()
    {
        _repository.Eliminar(Guid.NewGuid());
    }
}
