using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Dominio.Exceptions;

namespace Test.Domain;

[TestClass]
public class AtraccionTests
{
    [TestMethod]
    public void Constructor_Valido_DeberiaCrearAtraccion()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Divertida", true);
        Assert.IsNotNull(atraccion);
        Assert.AreEqual("Montaña Rusa", atraccion.Nombre);
        Assert.AreEqual(TipoAtraccion.MontañaRusa, atraccion.Tipo);
        Assert.AreEqual(12, atraccion.EdadMinima);
        Assert.AreEqual(20, atraccion.CapacidadMaxima);
        Assert.AreEqual("Divertida", atraccion.Descripcion);
        Assert.AreNotEqual(Guid.Empty, atraccion.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(AtraccionException))]
    public void Constructor_SinNombre_DeberiaFallar()
    {
        new Atraccion(string.Empty, TipoAtraccion.MontañaRusa, 12, 20, "Divertida", true);
    }

    [TestMethod]
    [ExpectedException(typeof(AtraccionException))]
    public void Constructor_EdadMinimaNegativa_DeberiaFallar()
    {
        new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, -1, 20, "Divertida", true);
    }

    [TestMethod]
    [ExpectedException(typeof(AtraccionException))]
    public void Constructor_CapacidadMaximaCero_DeberiaFallar()
    {
        new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 0, "Divertida", true);
    }

    [TestMethod]
    [ExpectedException(typeof(AtraccionException))]
    public void Constructor_DescripcionVacia_DeberiaFallar()
    {
        new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 20, string.Empty, true);
    }

    [TestMethod]
    public void Actualizar_TodosCampos_Valido()
    {
        var atraccion = new Atraccion("Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Divertida", true);

        atraccion.Actualizar("Nueva Rusa", TipoAtraccion.Simulador, 15, 25, "Más emocionante", true);

        Assert.AreEqual("Nueva Rusa", atraccion.Nombre);
        Assert.AreEqual(TipoAtraccion.Simulador, atraccion.Tipo);
        Assert.AreEqual(15, atraccion.EdadMinima);
        Assert.AreEqual(25, atraccion.CapacidadMaxima);
        Assert.AreEqual("Más emocionante", atraccion.Descripcion);
    }

    [TestMethod]
    [ExpectedException(typeof(AtraccionException))]
    public void Actualizar_NombreVacio_DeberiaFallar()
    {
        var atraccion = new Atraccion("Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Divertida", true);

        // Nombre vacío debería lanzar excepción
        atraccion.Actualizar(string.Empty, TipoAtraccion.MontañaRusa, 12, 20, "Divertida", true);
    }

    [TestMethod]
    [ExpectedException(typeof(AtraccionException))]
    public void Actualizar_EdadMinimaNegativa_DeberiaFallar()
    {
        var atraccion = new Atraccion("Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Divertida", true);

        atraccion.Actualizar("Rusa", TipoAtraccion.MontañaRusa, -1, 20, "Divertida", true);
    }

    [TestMethod]
    [ExpectedException(typeof(AtraccionException))]
    public void Actualizar_CapacidadMaximaInvalida_DeberiaFallar()
    {
        var atraccion = new Atraccion("Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Divertida", true);

        atraccion.Actualizar("Rusa", TipoAtraccion.MontañaRusa, 12, 0, "Divertida", true);
    }

    [TestMethod]
    [ExpectedException(typeof(AtraccionException))]
    public void Actualizar_DescripcionVacia_DeberiaFallar()
    {
        var atraccion = new Atraccion("Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Divertida", true);

        atraccion.Actualizar("Rusa", TipoAtraccion.MontañaRusa, 12, 20, string.Empty, true);
    }

    [TestMethod]
    public void Atraccion_Id_GetSet_DeberiaFuncionar()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 10, 20, "Emocionante", true);
        var nuevoId = Guid.NewGuid();
        atraccion.Id = nuevoId;
        Assert.AreEqual(nuevoId, atraccion.Id);
    }

    [TestMethod]
    public void PuedeIngresarVisitante_VisitanteCumpleEdadMinima_DeberiaRetornarTrue()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Emocionante", true);
        var visitante = new Visitante("Juan", "Perez", "juan@test.com", "Abcdef1!", new DateTime(2000, 1, 1),
            NivelMembresia.Estandar);
        var fechaActual = new DateTime(2025, 10, 8);

        var resultado = atraccion.PuedeIngresarVisitante(visitante, fechaActual);

        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void PuedeIngresarVisitante_VisitanteNoCumpleEdadMinima_DeberiaRetornarFalse()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Emocionante", true);
        var visitante = new Visitante("Niño", "Perez", "nino@test.com", "Abcdef1!", new DateTime(2020, 1, 1),
            NivelMembresia.Estandar);
        var fechaActual = new DateTime(2025, 10, 8);

        var resultado = atraccion.PuedeIngresarVisitante(visitante, fechaActual);

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void PuedeIngresarVisitante_VisitanteJustoCumpleEdad_DeberiaRetornarTrue()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Emocionante", true);
        var visitante = new Visitante("Adolescente", "Lopez", "adolescente@test.com", "Abcdef1!",
            new DateTime(2013, 10, 8), NivelMembresia.Estandar);
        var fechaActual = new DateTime(2025, 10, 8);

        var resultado = atraccion.PuedeIngresarVisitante(visitante, fechaActual);

        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void PuedeIngresarVisitante_VisitanteAunNoCumpleAños_DeberiaRetornarFalse()
    {
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Emocionante", true);
        var visitante = new Visitante("Casi12", "Garcia", "casi12@test.com", "Abcdef1!", new DateTime(2013, 12, 25),
            NivelMembresia.Estandar);
        var fechaActual = new DateTime(2025, 10, 8);

        var resultado = atraccion.PuedeIngresarVisitante(visitante, fechaActual);

        Assert.IsFalse(resultado);
    }
}
