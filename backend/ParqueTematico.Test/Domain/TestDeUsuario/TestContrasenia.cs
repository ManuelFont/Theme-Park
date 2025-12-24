using Dominio.Entities.Usuarios;
using Dominio.Exceptions;

namespace Test.Domain.TestDeUsuario;

[TestClass]
public class TestsContrasena
{
    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void Contrasena_null_lanza_excepcion()
    {
        _ = new Administrador("Juan", "Perez", "juan@example.com", null!);
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void Contrasena_vacia_lanza_excepcion()
    {
        _ = new Administrador("Juan", "Perez", "juan@example.com", string.Empty);
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void Contrasena_solo_espacios_lanza_excepcion()
    {
        _ = new Administrador("Juan", "Perez", "juan@example.com", "   ");
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void Contrasena_menos_de_8_caracteres_lanza_excepcion()
    {
        _ = new Administrador("Juan", "Perez", "juan@example.com", "Ab1!x7");
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void Contrasena_sin_mayuscula_lanza_excepcion()
    {
        _ = new Administrador("Juan", "Perez", "juan@example.com", "abcdef1!");
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void Contrasena_sin_minuscula_lanza_excepcion()
    {
        _ = new Administrador("Juan", "Perez", "juan@example.com", "ABCDEF1!");
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void Contrasena_sin_numero_lanza_excepcion()
    {
        _ = new Administrador("Juan", "Perez", "juan@example.com", "Abcdefg!");
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void Contrasena_sin_caracter_especial_lanza_excepcion()
    {
        _ = new Administrador("Juan", "Perez", "juan@example.com", "Abcdefg1");
    }

    [TestMethod]
    public void Contrasena_valida_se_asigna_y_valida_hash()
    {
        var u = new Administrador("Juan", "Admin", "ana@example.com", "Abcdef1!");
        Assert.IsNotNull(u.Contrasenia.Hash);
        Assert.AreNotEqual("Abcdef1!", u.Contrasenia.Hash);
        Assert.IsTrue(u.ValidarContrasenia("Abcdef1!"));
        Assert.IsFalse(u.ValidarContrasenia("OtraClave!"));
    }
}
