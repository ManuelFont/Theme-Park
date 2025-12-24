using Dominio.Entities;
using Dominio.Exceptions;

namespace Test.Domain;

[TestClass]
public class EventoTests
{
    private Atraccion? _atraccion1;
    private Atraccion? _atraccion2;
    private Evento? _evento;

    [TestInitialize]
    public void Setup()
    {
        _evento = new Evento(
            "Noche de Dinosaurios",
            new DateTime(2025, 12, 24),
            new TimeSpan(18, 0, 0),
            100,
            50m);

        _atraccion1 = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 10, 20, "Emocionante", true);
        _atraccion2 = new Atraccion("Simulador", TipoAtraccion.Simulador, 8, 15, "Virtual", true);
    }

    [TestMethod]
    public void Evento_Creacion_DeberiaTenerPropiedadesCorrectas()
    {
        Assert.AreEqual("Noche de Dinosaurios", _evento?.Nombre);
        if(_evento != null)
        {
            Assert.AreEqual(new DateTime(2025, 12, 24), _evento.Fecha);
            Assert.AreEqual(new TimeSpan(18, 0, 0), _evento.Hora);
            Assert.AreEqual(100, _evento.Aforo);
            Assert.AreEqual(50m, _evento.CostoAdicional);
            Assert.AreEqual(0, _evento.Atracciones.Count);
        }
    }

    [TestMethod]
    public void Evento_AgregarAtraccion_DeberiaSumarAtraccion()
    {
        if(_atraccion1 != null)
        {
            _evento!.Atracciones.Add(_atraccion1);
            Assert.AreEqual(1, _evento.Atracciones.Count);
            Assert.AreEqual(_atraccion1, _evento.Atracciones[0]);
        }
    }

    [TestMethod]
    public void Evento_RemoverAtraccion_DeberiaEliminarAtraccion()
    {
        if(_atraccion1 != null)
        {
            _evento!.Atracciones.Add(_atraccion1);
            if(_atraccion2 != null)
            {
                _evento.Atracciones.Add(_atraccion2);
            }

            _evento.Atracciones.Remove(_atraccion1);
        }

        if(_evento != null)
        {
            Assert.AreEqual(1, _evento.Atracciones.Count);
            Assert.AreEqual(_atraccion2, _evento.Atracciones[0]);
        }
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void Evento_NombreVacio_DeberiaLanzarExcepcion()
    {
        new Evento(string.Empty, DateTime.Now.AddDays(1), TimeSpan.Zero, 10, 0);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void Evento_FechaPasada_DeberiaLanzarExcepcion()
    {
        new Evento("Evento", DateTime.Now.AddDays(-1), TimeSpan.Zero, 10, 0);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void Evento_AforoCero_DeberiaLanzarExcepcion()
    {
        new Evento("Evento", DateTime.Now.AddDays(1), TimeSpan.Zero, 0, 0);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void Evento_CostoNegativo_DeberiaLanzarExcepcion()
    {
        new Evento("Evento", DateTime.Now.AddDays(1), TimeSpan.Zero, 10, -1);
    }

    [TestMethod]
    public void Evento_Atracciones_GetSet_DeberiaFuncionar()
    {
        var nuevaLista = new List<Atraccion>();
        if(_evento != null)
        {
            _evento.Atracciones = nuevaLista;
            Assert.AreEqual(nuevaLista, _evento.Atracciones);
        }
    }

    [TestMethod]
    public void CrearEvento_ConDatosValidos_DeberiaCrearse()
    {
        var evento = new Evento("Festival", new DateTime(2025, 12, 1), new TimeSpan(18, 0, 0), 100, 500);

        Assert.AreEqual("Festival", evento.Nombre);
        Assert.AreEqual(new DateTime(2025, 12, 1), evento.Fecha);
        Assert.AreEqual(new TimeSpan(18, 0, 0), evento.Hora);
        Assert.AreEqual(100, evento.Aforo);
        Assert.AreEqual(500, evento.CostoAdicional);
        Assert.AreEqual(0, evento.Atracciones.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void CrearEvento_ConNombreVacio_DeberiaTirarExcepcion()
    {
        new Evento(string.Empty, new DateTime(2025, 12, 1), new TimeSpan(18, 0, 0), 100, 500);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void CrearEvento_ConNombreNull_DeberiaTirarExcepcion()
    {
        new Evento(null!, new DateTime(2025, 12, 1), new TimeSpan(18, 0, 0), 100, 500);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void CrearEvento_ConFechaPasada_DeberiaTirarExcepcion()
    {
        new Evento("Festival", DateTime.Now.AddDays(-1), new TimeSpan(18, 0, 0), 100, 500);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void CrearEvento_ConAforoCero_DeberiaTirarExcepcion()
    {
        new Evento("Festival", new DateTime(2025, 12, 1), new TimeSpan(18, 0, 0), 0, 500);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void CrearEvento_ConAforoNegativo_DeberiaTirarExcepcion()
    {
        new Evento("Festival", new DateTime(2025, 12, 1), new TimeSpan(18, 0, 0), -10, 500);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void CrearEvento_ConCostoNegativo_DeberiaTirarExcepcion()
    {
        new Evento("Festival", new DateTime(2025, 12, 1), new TimeSpan(18, 0, 0), 100, -50);
    }

    [TestMethod]
    public void AgregarAtraccion_DeberiaAgregarla()
    {
        var evento = new Evento("Festival", new DateTime(2025, 12, 1), new TimeSpan(18, 0, 0), 100, 500);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);

        evento.AgregarAtraccion(atraccion);

        Assert.AreEqual(1, evento.Atracciones.Count);
        Assert.AreEqual("Montaña Rusa", evento.Atracciones[0].Nombre);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void AgregarAtraccion_ConAtraccionNull_DeberiaTirarExcepcion()
    {
        var evento = new Evento("Festival", new DateTime(2025, 12, 1), new TimeSpan(18, 0, 0), 100, 500);

        evento.AgregarAtraccion(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void AgregarAtraccion_AtraccionYaAsignada_DeberiaTirarExcepcion()
    {
        var evento = new Evento("Festival", new DateTime(2025, 12, 1), new TimeSpan(18, 0, 0), 100, 500);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);

        evento.AgregarAtraccion(atraccion);
        evento.AgregarAtraccion(atraccion);
    }

    [TestMethod]
    public void AgregarAtraccion_VariasAtracciones_DeberiaAgregarTodas()
    {
        var evento = new Evento("Festival", new DateTime(2025, 12, 1), new TimeSpan(18, 0, 0), 100, 500);
        var atraccion1 = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción 1", true);
        var atraccion2 = new Atraccion("Simulador VR", TipoAtraccion.Simulador, 3, 25, "Descripción 2", true);
        var atraccion3 = new Atraccion("Show Musical", TipoAtraccion.Espectáculo, 10, 15, "Descripción 3", true);

        evento.AgregarAtraccion(atraccion1);
        evento.AgregarAtraccion(atraccion2);
        evento.AgregarAtraccion(atraccion3);

        Assert.AreEqual(3, evento.Atracciones.Count);
    }

    [TestMethod]
    public void EliminarAtraccion_DeberiaEliminarla()
    {
        var evento = new Evento("Festival", new DateTime(2025, 12, 1), new TimeSpan(18, 0, 0), 100, 500);
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción", true);

        evento.AgregarAtraccion(atraccion);
        evento.EliminarAtraccion(atraccion.Id);

        Assert.AreEqual(0, evento.Atracciones.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(EventoException))]
    public void EliminarAtraccion_AtraccionNoAsignada_DeberiaTirarExcepcion()
    {
        var evento = new Evento("Festival", new DateTime(2025, 12, 1), new TimeSpan(18, 0, 0), 100, 500);

        evento.EliminarAtraccion(Guid.NewGuid());
    }

    [TestMethod]
    public void EliminarAtraccion_UnaDeVarias_DeberiaSoloEliminarUna()
    {
        var evento = new Evento("Festival", new DateTime(2025, 12, 1), new TimeSpan(18, 0, 0), 100, 500);
        var atraccion1 = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Descripción 1", true);
        var atraccion2 = new Atraccion("Simulador VR", TipoAtraccion.Simulador, 3, 25, "Descripción 2", true);

        evento.AgregarAtraccion(atraccion1);
        evento.AgregarAtraccion(atraccion2);
        evento.EliminarAtraccion(atraccion1.Id);

        Assert.AreEqual(1, evento.Atracciones.Count);
        Assert.AreEqual("Simulador VR", evento.Atracciones[0].Nombre);
    }
}
