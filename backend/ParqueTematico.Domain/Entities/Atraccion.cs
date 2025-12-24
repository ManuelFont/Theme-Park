using Dominio.Entities.Usuarios;
using Dominio.Exceptions;

namespace Dominio.Entities;

public enum TipoAtraccion
{
    /// <summary>
    ///     Montaña rusa clásica con loops y velocidad.
    /// </summary>
    MontañaRusa,

    /// <summary>
    ///     Simulador virtual o físico de experiencias.
    /// </summary>
    Simulador,

    /// <summary>
    ///     Espectáculos en vivo o presentaciones especiales.
    /// </summary>
    Espectáculo,

    /// <summary>
    ///     Atracciones interactivas para todas las edades.
    /// </summary>
    ZonaInteractiva,
}

public class Atraccion
{
    public Atraccion(
        string nombre,
        TipoAtraccion tipo,
        int edadMinima,
        int capacidadMaxima,
        string descripcion,
        bool disponible)
    {
        if(string.IsNullOrEmpty(nombre))
        {
            throw new AtraccionException("El nombre no puede ser vacío");
        }

        if(edadMinima < 0)
        {
            throw new AtraccionException("La edad minima no puede ser negativa");
        }

        if(capacidadMaxima < 1)
        {
            throw new AtraccionException("La capacidad maxima no puede ser cero o negativa");
        }

        if(string.IsNullOrEmpty(descripcion))
        {
            throw new AtraccionException("La descripción no puede ser vacía");
        }

        Nombre = nombre;
        Tipo = tipo;
        EdadMinima = edadMinima;
        CapacidadMaxima = capacidadMaxima;
        Descripcion = descripcion;
        Disponible = disponible;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nombre { get; private set; }
    public TipoAtraccion Tipo { get; private set; }
    public int EdadMinima { get; private set; }
    public int CapacidadMaxima { get; private set; }
    public string Descripcion { get; private set; }
    public bool Disponible { get; set; }
    public List<Incidencia> Incidencias { get; set; } = [];

    public void Actualizar(
        string nombre,
        TipoAtraccion tipo,
        int edadMinima,
        int capacidadMaxima,
        string descripcion,
        bool disponible)
    {
        if(string.IsNullOrEmpty(nombre))
        {
            throw new AtraccionException("El nombre no puede ser vacío");
        }

        if(edadMinima < 0)
        {
            throw new AtraccionException("La edad minima no puede ser negativa");
        }

        if(capacidadMaxima < 1)
        {
            throw new AtraccionException("La capacidad maxima no puede ser cero o negativa");
        }

        if(string.IsNullOrEmpty(descripcion))
        {
            throw new AtraccionException("La descripción no puede ser vacía");
        }

        Nombre = nombre;
        Tipo = tipo;
        EdadMinima = edadMinima;
        CapacidadMaxima = capacidadMaxima;
        Descripcion = descripcion;
        Disponible = disponible;
    }

    public bool PuedeIngresarVisitante(Visitante visitante, DateTime fechaActual)
    {
        var edad = CalcularEdad(visitante.FechaNacimiento, fechaActual);
        return edad >= EdadMinima;
    }

    private int CalcularEdad(DateTime fechaNacimiento, DateTime fechaActual)
    {
        var edad = fechaActual.Year - fechaNacimiento.Year;
        if(fechaActual < fechaNacimiento.AddYears(edad))
        {
            edad--;
        }

        return edad;
    }
}
