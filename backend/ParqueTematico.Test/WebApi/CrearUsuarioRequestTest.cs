using Dominio.Entities.Usuarios;
using Dtos;

namespace Test.WebApi;

[TestClass]
public class CrearUsuarioRequestTest
{
    [TestMethod]
    public void CrearUsuarioRequest_SettersYGetters_CubrenTodo()
    {
        var request = new CrearUsuarioRequest
        {
            Nombre = "Nahuel",
            Apellido = "Mileo",
            Email = "nahuel@mail.com",
            Contrasenia = "Pass123!",
            TipoUsuario = "visitante",
            FechaNacimiento = new DateTime(2000, 1, 1),
            NivelMembresia = NivelMembresia.Premium
        };

        Assert.AreEqual("Nahuel", request.Nombre);
        Assert.AreEqual("Mileo", request.Apellido);
        Assert.AreEqual("nahuel@mail.com", request.Email);
        Assert.AreEqual("Pass123!", request.Contrasenia);
        Assert.AreEqual("visitante", request.TipoUsuario);
        Assert.AreEqual(new DateTime(2000, 1, 1), request.FechaNacimiento);
        Assert.AreEqual(NivelMembresia.Premium, request.NivelMembresia);

        request.Nombre = "Manuel";
        request.Apellido = "Font";
        request.Email = "mail@mail.com";
        request.Contrasenia = "NuevaPass!";
        request.TipoUsuario = "administrador";
        request.FechaNacimiento = new DateTime(1995, 5, 5);
        request.NivelMembresia = NivelMembresia.Estandar;

        Assert.AreEqual("Manuel", request.Nombre);
        Assert.AreEqual("Font", request.Apellido);
        Assert.AreEqual("mail@mail.com", request.Email);
        Assert.AreEqual("NuevaPass!", request.Contrasenia);
        Assert.AreEqual("administrador", request.TipoUsuario);
        Assert.AreEqual(new DateTime(1995, 5, 5), request.FechaNacimiento);
        Assert.AreEqual(NivelMembresia.Estandar, request.NivelMembresia);
    }
}
