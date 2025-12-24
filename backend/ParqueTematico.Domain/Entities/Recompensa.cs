using Dominio.Entities.Usuarios;

namespace Dominio.Entities;

public class Recompensa
{
    public Recompensa(string nombre, string descripcion, int costo, int cantidadDisponible,
        NivelMembresia? nivelRequerido)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        Costo = costo;
        CantidadDisponible = cantidadDisponible;
        NivelRequerido = nivelRequerido;
        ValidarDatosSonValidos();
    }

    public Recompensa(string nombre, string descripcion, int costo, int cantidadDisponible)
        : this(nombre, descripcion, costo, cantidadDisponible, null)
    {
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int Costo { get; set; }
    public int CantidadDisponible { get; set; }
    public NivelMembresia? NivelRequerido { get; set; }

    private void ValidarDatosSonValidos()
    {
        if(string.IsNullOrWhiteSpace(Nombre))
        {
            throw new ArgumentException("Recompensa debe tener un nombre");
        }

        if(string.IsNullOrWhiteSpace(Descripcion))
        {
            throw new ArgumentException("Recompensa debe tener una Descripcion");
        }

        if(Costo < 0)
        {
            throw new ArgumentException("Costo no puede ser menor a cero");
        }

        if(CantidadDisponible < 0)
        {
            throw new ArgumentException("CantidadDisponible no puede ser menor a cero");
        }
    }
}
