using Dominio.Entities.Usuarios;
using Dominio.Exceptions;
using Dtos;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repositories;
using ParqueTematico.Application.Services;

namespace Test.Application;

[TestClass]
public class UsuarioServiceTests
{
    private ParqueDbContext _context = null!;
    private UsuarioDbRepository _repository = null!;
    private CrearUsuarioRequest _request1 = null!;
    private CrearUsuarioRequest _request2 = null!;
    private Usuario _visitante1 = null!;
    private Usuario _visitante2 = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = SqlContextFactory.CreateMemoryContext();
        _repository = new UsuarioDbRepository(_context);
        var visitanteFechaNac1 = new DateTime(2004, 07, 21);
        _visitante1 = new Visitante("manuel", "font", "mail@mail.com", "Pass123!", visitanteFechaNac1,
            NivelMembresia.Estandar);
        _visitante2 = new Visitante("manuel", "font", "maill@mail.com", "Pass123!", visitanteFechaNac1,
            NivelMembresia.Estandar);
        _request1 = new CrearUsuarioRequest
        {
            Nombre = _visitante1.Nombre,
            Apellido = _visitante1.Apellido,
            Email = _visitante1.Email,
            Contrasenia = "Pass123!",
            TipoUsuario = "visitante",
            FechaNacimiento = new DateTime(2004, 07, 21),
            NivelMembresia = 0
        };

        _request2 = new CrearUsuarioRequest
        {
            Nombre = _visitante2.Nombre,
            Apellido = _visitante2.Apellido,
            Email = _visitante2.Email,
            Contrasenia = "Pass123!",
            TipoUsuario = "visitante",
            FechaNacimiento = new DateTime(2004, 07, 21),
            NivelMembresia = 0
        };
    }

    [TestMethod]
    public void InicializarAdministrador_YaExiste_NoSeInicializa()
    {
        var service = new UsuarioService(_repository);
        service.InicializarAdministrador();
        service.InicializarAdministrador();
        Assert.AreEqual(1, _repository.ObtenerTodos().Count);
    }

    [TestMethod]
    public void UsuarioService_ParamCorrectos_ObjetoCreado()
    {
        var service = new UsuarioService(_repository);
        Assert.IsNotNull(service);
    }

    [TestMethod]
    public void CrearUsuario_DatosCorrectos_UsuarioCreado()
    {
        var service = new UsuarioService(_repository);
        UsuarioDTO usuarioDTO = service.CrearUsuario(_request1);

        Usuario? usuarioGuardado = _repository.ObtenerTodos().FirstOrDefault();
        Assert.AreEqual(usuarioGuardado!.Id, usuarioDTO.Id);
    }

    [TestMethod]
    public void ListarUsuarios_DevuelveTodosLosUsuarios()
    {
        var service = new UsuarioService(_repository);
        UsuarioDTO u1 = service.CrearUsuario(_request1);
        UsuarioDTO u2 = service.CrearUsuario(_request2);
        IList<UsuarioDTO> lista = service.ListarUsuarios();

        Assert.AreEqual(2, lista.Count);
        Assert.IsTrue(lista.Any(u => u.Id == u1.Id));
        Assert.IsTrue(lista.Any(u => u.Id == u2.Id));
    }

    [TestMethod]
    public void ObtenerUsuarioPorId_DevuelveUsuario()
    {
        var service = new UsuarioService(_repository);
        UsuarioDTO usuario = service.CrearUsuario(_request1);
        UsuarioDTO? u = service.ObtenerPorId(usuario.Id);
        Assert.IsNotNull(u);
        Assert.AreEqual(usuario.Id, u.Id);
    }

    [TestMethod]
    public void ObtenerUsuarioPorId_DevuelveNulSiUsuarioNoExiste()
    {
        var service = new UsuarioService(_repository);
        UsuarioDTO? u = service.ObtenerPorId(Guid.NewGuid());
        Assert.IsNull(u);
    }

    [TestMethod]
    public void Actualizar_UsuarioExistente_DebeActualizarCampos()
    {
        var service = new UsuarioService(_repository);
        UsuarioDTO usuario = service.CrearUsuario(_request1);

        var requestActualizar = new ActualizarUsuarioRequest
        {
            Nombre = "NuevoNombre",
            Apellido = "NuevoApellido",
            Email = "nuevo@mail.com",
            Contrasenia = "NuevaPass123!",
            FechaNacimiento = new DateTime(2000, 01, 01),
            NivelMembresia = NivelMembresia.Premium
        };

        service.Actualizar(usuario.Id, requestActualizar);
        UsuarioDTO? usuarioActualizado = service.ObtenerPorId(usuario.Id);

        Assert.IsNotNull(usuarioActualizado);
        Assert.AreEqual("NuevoNombre", usuarioActualizado.Nombre);
        Assert.AreEqual("NuevoApellido", usuarioActualizado.Apellido);
        Assert.AreEqual("nuevo@mail.com", usuarioActualizado.Email);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Actualizar_UsuarioNoExiste_DebeLanzarExcepcion()
    {
        var service = new UsuarioService(_repository);
        var request = new ActualizarUsuarioRequest
        {
            Nombre = "X",
            Apellido = "Y",
            Email = "z@mail.com",
            Contrasenia = "Pass1!",
            FechaNacimiento = DateTime.Now,
            NivelMembresia = NivelMembresia.Estandar
        };

        service.Actualizar(Guid.NewGuid(), request);
    }

    [TestMethod]
    public void Eliminar_UsuarioExistente_DebeEliminarUsuario()
    {
        var service = new UsuarioService(_repository);
        UsuarioDTO usuario = service.CrearUsuario(_request1);

        service.Eliminar(usuario.Id);
        UsuarioDTO? eliminado = service.ObtenerPorId(usuario.Id);

        Assert.IsNull(eliminado);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void Eliminar_UsuarioNoExiste_DebeLanzarExcepcion()
    {
        var service = new UsuarioService(_repository);

        service.Eliminar(Guid.NewGuid());
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void Actualizar_VisitanteSinFechaNacimientoONivelMembresia_DebeLanzarExcepcion()
    {
        var service = new UsuarioService(_repository);
        UsuarioDTO usuario = service.CrearUsuario(_request1);

        var requestActualizar = new ActualizarUsuarioRequest
        {
            Nombre = "Test",
            Apellido = "Test",
            Email = "test@mail.com",
            Contrasenia = "Pass123!",
            FechaNacimiento = null,
            NivelMembresia = null
        };

        service.Actualizar(usuario.Id, requestActualizar);
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void CrearUsuario_VisitanteSinFechaNacimiento_DebeLanzarExcepcion()
    {
        var service = new UsuarioService(_repository);
        var request = new CrearUsuarioRequest
        {
            Nombre = "Visitante",
            Apellido = "Test",
            Email = "visitante@mail.com",
            Contrasenia = "Pass123!",
            TipoUsuario = "visitante",
            NivelMembresia = NivelMembresia.Estandar
        };

        service.CrearUsuario(request);
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void CrearUsuario_VisitanteSinNivelMembresia_DebeLanzarExcepcion()
    {
        var service = new UsuarioService(_repository);
        var request = new CrearUsuarioRequest
        {
            Nombre = "Ana",
            Apellido = "Gomez",
            Email = "ana@mail.com",
            Contrasenia = "Pass123!",
            TipoUsuario = "visitante",
            FechaNacimiento = DateTime.Now
        };

        service.CrearUsuario(request);
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void CrearUsuario_TipoUsuarioInvalido_DebeLanzarExcepcion()
    {
        var service = new UsuarioService(_repository);
        var request = new CrearUsuarioRequest
        {
            Nombre = "Pepe",
            Apellido = "Loco",
            Email = "pepe@mail.com",
            Contrasenia = "Pass123!",
            TipoUsuario = "desconocido"
        };

        service.CrearUsuario(request);
    }

    [TestMethod]
    public void CrearUsuario_TipoAdministrador_UsuarioCreado()
    {
        var service = new UsuarioService(_repository);
        var request = new CrearUsuarioRequest
        {
            Nombre = "Admin",
            Apellido = "Test",
            Email = "admin@mail.com",
            Contrasenia = "Pass123!",
            TipoUsuario = "administrador"
        };

        UsuarioDTO usuarioDTO = service.CrearUsuario(request);

        Assert.IsNotNull(usuarioDTO);
        Assert.AreEqual("Admin", usuarioDTO.Nombre);
        Assert.AreEqual("admin@mail.com", usuarioDTO.Email);
    }

    [TestMethod]
    public void CrearUsuario_TipoOperador_UsuarioCreado()
    {
        var service = new UsuarioService(_repository);
        var request = new CrearUsuarioRequest
        {
            Nombre = "Operador",
            Apellido = "Test",
            Email = "operador@mail.com",
            Contrasenia = "Pass123!",
            TipoUsuario = "operador"
        };

        UsuarioDTO usuarioDTO = service.CrearUsuario(request);

        Assert.IsNotNull(usuarioDTO);
        Assert.AreEqual("Operador", usuarioDTO.Nombre);
        Assert.AreEqual("operador@mail.com", usuarioDTO.Email);
    }

    [TestMethod]
    public void ObtenerIdPorEmail_EmailValido_DevuelveId()
    {
        var service = new UsuarioService(_repository);
        UsuarioDTO usuario = service.CrearUsuario(_request1);

        Guid usuarioId = service.ObtenerIdPorEmail(_visitante1.Email);

        Assert.AreEqual(usuario.Id, usuarioId);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void ObtenerIdPorEmail_EmailNoExiste_LanzaExcepcion()
    {
        var service = new UsuarioService(_repository);

        service.ObtenerIdPorEmail("noexiste@mail.com");
    }

    [TestMethod]
    public void ObtenerTipoPorEmail_EmailValido_DevuelveTipo()
    {
        var service = new UsuarioService(_repository);
        service.CrearUsuario(_request1);

        var tipo = service.ObtenerTipoPorEmail(_visitante1.Email);

        Assert.AreEqual("Visitante", tipo);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void ObtenerTipoPorEmail_EmailNoExiste_LanzaExcepcion()
    {
        var service = new UsuarioService(_repository);

        service.ObtenerTipoPorEmail("noexiste@mail.com");
    }
}
