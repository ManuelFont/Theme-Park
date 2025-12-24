using Dominio.Entities.Usuarios;
using Dominio.Exceptions;

namespace Test.Domain.TestDeUsuario;

[TestClass]
public class TestVisitanteProps
{
    [TestMethod]
    public void Visitante_NfcId_DebeGenerarse_NoVacio()
    {
        var v = new Visitante("Juan", "Perez", "juan@example.com", "Abcdef1!", new DateTime(2000, 5, 10),
            NivelMembresia.Estandar);
        Assert.AreNotEqual(Guid.Empty, v.NfcId);
    }

    [TestMethod]
    public void Visitante_NfcId_DebeSerDistinto_EntreInstancias()
    {
        var v1 = new Visitante("Ana", "Suarez", "ana@example.com", "Abcdef1!", new DateTime(1999, 1, 1),
            NivelMembresia.Estandar);
        var v2 = new Visitante("Luis", "Gomez", "luis@example.com", "Abcdef1!", new DateTime(1998, 2, 2),
            NivelMembresia.Estandar);
        Assert.AreNotEqual(v1.NfcId, v2.NfcId);
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void Visitante_FechaNacimiento_Futura_DebeLanzarExcepcion()
    {
        DateTime futura = DateTime.UtcNow.Date.AddDays(1);
        _ = new Visitante("Carla", "Diaz", "carla@example.com", "Abcdef1!", futura, NivelMembresia.Estandar);
    }

    [TestMethod]
    public void Visitante_FechaNacimiento_Hoy_EsValida()
    {
        DateTime hoy = DateTime.UtcNow.Date;
        var v = new Visitante("Beto", "Lopez", "beto@example.com", "Abcdef1!", hoy, NivelMembresia.Premium);
        Assert.AreEqual(hoy, v.FechaNacimiento);
    }

    [TestMethod]
    public void Visitante_FechaNacimiento_Pasada_EsValida()
    {
        var pasada = new DateTime(1990, 12, 1);
        var v = new Visitante("Nico", "Ramos", "nico@example.com", "Abcdef1!", pasada, NivelMembresia.Vip);
        Assert.AreEqual(pasada, v.FechaNacimiento);
    }

    [TestMethod]
    public void Visitante_NivelMembresia_SeAsignaEnConstructor_YCambiaSoloPorMetodo()
    {
        var v = new Visitante("Mario", "Rossi", "mario@example.com", "Abcdef1!", new DateTime(2001, 7, 7),
            NivelMembresia.Estandar);
        Assert.AreEqual(NivelMembresia.Estandar, v.NivelMembresia);
        v.CambiarMembresiaPorAdministracion(NivelMembresia.Premium);
        Assert.AreEqual(NivelMembresia.Premium, v.NivelMembresia);
    }

    [TestMethod]
    public void EdadEn_DeberiaCalcularEdadCorrecta()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 6, 15),
            NivelMembresia.Estandar);
        var fechaConsulta = new DateTime(2025, 10, 8);

        var edad = visitante.EdadEn(fechaConsulta);

        Assert.AreEqual(25, edad);
    }

    [TestMethod]
    public void EdadEn_AntesDelCumpleaños_DeberiaRestarUnAño()
    {
        var visitante = new Visitante("Maria", "Lopez", "maria@test.com", "Abcdef1!", new DateTime(2000, 12, 25),
            NivelMembresia.Estandar);
        var fechaConsulta = new DateTime(2025, 10, 8);

        var edad = visitante.EdadEn(fechaConsulta);

        Assert.AreEqual(24, edad);
    }

    [TestMethod]
    public void EdadEn_MismoDiaCumpleaños_DeberiaContarAñoCompleto()
    {
        var visitante = new Visitante("Pedro", "Garcia", "pedro@test.com", "Abcdef1!", new DateTime(2000, 10, 8),
            NivelMembresia.Estandar);
        var fechaConsulta = new DateTime(2025, 10, 8);

        var edad = visitante.EdadEn(fechaConsulta);

        Assert.AreEqual(24, edad);
    }

    [TestMethod]
    public void PuntosActuales_AlCrearVisitante_DeberiaSerCero()
    {
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);

        Assert.AreEqual(0, visitante.PuntosActuales);
    }

    [TestMethod]
    public void AgregarPuntos_PuntosPositivos_DeberiaIncrementarPuntosActuales()
    {
        var visitante = new Visitante("Maria", "Lopez", "maria@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);

        visitante.AgregarPuntos(100);

        Assert.AreEqual(100, visitante.PuntosActuales);
    }

    [TestMethod]
    public void AgregarPuntos_MultiplesVeces_DeberiaSumarPuntos()
    {
        var visitante = new Visitante("Pedro", "Garcia", "pedro@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);

        visitante.AgregarPuntos(50);
        visitante.AgregarPuntos(75);

        Assert.AreEqual(125, visitante.PuntosActuales);
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void AgregarPuntos_PuntosNegativos_DebeLanzarExcepcion()
    {
        var visitante = new Visitante("Ana", "Suarez", "ana@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);

        visitante.AgregarPuntos(-10);
    }

    [TestMethod]
    public void DescontarPuntos_PuntosDisponibles_DeberiaReducirPuntosActuales()
    {
        var visitante = new Visitante("Luis", "Gomez", "luis@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        visitante.AgregarPuntos(200);

        visitante.DescontarPuntos(50);

        Assert.AreEqual(150, visitante.PuntosActuales);
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void DescontarPuntos_PuntosInsuficientes_DebeLanzarExcepcion()
    {
        var visitante = new Visitante("Carlos", "Diaz", "carlos@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        visitante.AgregarPuntos(50);

        visitante.DescontarPuntos(100);
    }

    [TestMethod]
    [ExpectedException(typeof(UserException))]
    public void DescontarPuntos_PuntosNegativos_DebeLanzarExcepcion()
    {
        var visitante = new Visitante("Sofia", "Ramos", "sofia@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);

        visitante.DescontarPuntos(-20);
    }
}
