using Dominio.Entities.Usuarios;
using Dtos;

namespace Test.Dtos;

[TestClass]
public class ActualizarUsuarioRequestTests
{
    [TestMethod]
    public void ActualizarUsuarioRequest_SettersYGetters_DebenFuncionar()
    {
        var fechaNacimiento = new DateTime(1995, 5, 5);

        var request = new ActualizarUsuarioRequest
        {
            Nombre = "Nahuel",
            Apellido = "Mileo",
            Email = "nahuel@mail.com",
            Contrasenia = "Pass123!",
            FechaNacimiento = fechaNacimiento,
            NivelMembresia = NivelMembresia.Premium
        };

        Assert.AreEqual("Nahuel", request.Nombre);
        Assert.AreEqual("Mileo", request.Apellido);
        Assert.AreEqual("nahuel@mail.com", request.Email);
        Assert.AreEqual("Pass123!", request.Contrasenia);
        Assert.AreEqual(fechaNacimiento, request.FechaNacimiento);
        Assert.AreEqual(NivelMembresia.Premium, request.NivelMembresia);

        request.Nombre = "Manuel";
        request.Apellido = "Font";
        request.Email = "otro@mail.com";
        request.Contrasenia = "NuevaPass!";
        request.FechaNacimiento = new DateTime(2000, 1, 1);
        request.NivelMembresia = NivelMembresia.Estandar;

        Assert.AreEqual("Manuel", request.Nombre);
        Assert.AreEqual("Font", request.Apellido);
        Assert.AreEqual("otro@mail.com", request.Email);
        Assert.AreEqual("NuevaPass!", request.Contrasenia);
        Assert.AreEqual(new DateTime(2000, 1, 1), request.FechaNacimiento);
        Assert.AreEqual(NivelMembresia.Estandar, request.NivelMembresia);
    }
}
