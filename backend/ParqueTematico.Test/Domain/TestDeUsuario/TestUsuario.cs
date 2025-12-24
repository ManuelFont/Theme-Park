using Dominio.Entities.Usuarios;
using Dominio.Exceptions;

namespace Test.Domain.TestDeUsuario;

[TestClass]
public class TestUsuario
{
    [TestMethod]
    public void Administrador_Creacion_DeberiaTenerPropiedadesCorrectas()
    {
        var admin = new Administrador("Juan", "Perez", "juan@example.com", "Abcdef1!");

        Assert.AreEqual("Juan", admin.Nombre);
        Assert.AreEqual("Perez", admin.Apellido);
        Assert.AreEqual("juan@example.com", admin.Email);
        Assert.IsNotNull(admin.Contrasenia.Hash);
        Assert.IsTrue(admin.ValidarContrasenia("Abcdef1!"));
        Assert.IsFalse(admin.ValidarContrasenia("OtraClave!"));
    }

    [TestMethod]
    public void Operador_Creacion_DeberiaTenerPropiedadesCorrectas()
    {
        var operador = new Operador("Juan", "Perez", "maria@example.com", "Abcdef1!");

        Assert.AreEqual("Juan", operador.Nombre);
        Assert.AreEqual("Perez", operador.Apellido);
        Assert.AreEqual("maria@example.com", operador.Email);
        Assert.IsNotNull(operador.Contrasenia.Hash);
        Assert.IsTrue(operador.ValidarContrasenia("Abcdef1!"));
        Assert.IsFalse(operador.ValidarContrasenia("OtraClave!"));
    }

    [TestMethod]
    public void Visitante_Creacion_DeberiaTenerPropiedadesCorrectas()
    {
        var fecha = new DateTime(2025, 9, 10);
        var visitante = new Visitante("Juan", "Perez", "carlos@example.com", "Abcdef1!", fecha, NivelMembresia.Premium);

        Assert.AreEqual("Juan", visitante.Nombre);
        Assert.AreEqual("Perez", visitante.Apellido);
        Assert.AreEqual("carlos@example.com", visitante.Email);
        Assert.IsNotNull(visitante.Contrasenia.Hash);
        Assert.IsTrue(visitante.ValidarContrasenia("Abcdef1!"));
        Assert.IsFalse(visitante.ValidarContrasenia("OtraClave!"));
        Assert.AreEqual(fecha, visitante.FechaNacimiento);
        Assert.AreEqual(NivelMembresia.Premium, visitante.NivelMembresia);
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void User_NombreVacio_DeberiaLanzarExcepcion()
    {
        _ = new Administrador(string.Empty, string.Empty, "juan@example.com", "Abcdef1!");
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void User_EmailVacio_DeberiaLanzarExcepcion()
    {
        _ = new Operador("Juan", "Perez", string.Empty, "Abcdef1!");
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void User_ContraseniaVacia_DeberiaLanzarExcepcion()
    {
        _ = new Visitante("Juan", "Perez", "carlos@example.com", string.Empty, DateTime.Now, NivelMembresia.Estandar);
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void User_EmailSinArroba_DeberiaLanzarExcepcion()
    {
        _ = new Administrador("Juan", "Perez", "juanexample.com", "Abcdef1!");
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void User_EmailSinDominio_DeberiaLanzarExcepcion()
    {
        _ = new Administrador("Juan", "Perez", "juan@", "Abcdef1!");
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void User_EmailConEspaciosInternos_DeberiaLanzarExcepcion()
    {
        _ = new Administrador("Juan", "Perez", "juan @example.com", "Abcdef1!");
    }

    [TestMethod]
    public void User_EmailMayusculas_DeberiaNormalizarseAMinusculas()
    {
        var admin = new Administrador("Juan", "Perez", "JuAn@Example.com", "Abcdef1!");
        Assert.AreEqual("juan@example.com", admin.Email);
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void Usuario_NombreVacio_DeberiaLanzarExcepcion()
    {
        _ = new Administrador("   ", "Perez", "juan@example.com", "Abcdef1!");
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void Usuario_ApellidoVacio_DeberiaLanzarExcepcion()
    {
        _ = new Administrador("Juan", "   ", "juan@example.com", "Abcdef1!");
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void Usuario_NombreMuyLargo_DeberiaLanzarExcepcion()
    {
        var nombreLargo = new string('a', 51);
        _ = new Administrador(nombreLargo, "Perez", "juan@example.com", "Abcdef1!");
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void Usuario_ApellidoMuyLargo_DeberiaLanzarExcepcion()
    {
        var apellidoLargo = new string('b', 51);
        _ = new Administrador("Juan", apellidoLargo, "juan@example.com", "Abcdef1!");
    }

    [TestMethod]
    public void Usuario_NombreYApellidoConEspacios_DeberianGuardarseLimpios()
    {
        var admin = new Administrador("  Juan  ", "  Perez  ", "juan@example.com", "Abcdef1!");
        Assert.AreEqual("Juan", admin.Nombre);
        Assert.AreEqual("Perez", admin.Apellido);
    }
}
