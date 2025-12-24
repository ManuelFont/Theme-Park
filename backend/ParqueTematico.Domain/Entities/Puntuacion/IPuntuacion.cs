namespace Dominio.Entities.Puntuacion;

public interface IPuntuacion
{
    string Nombre { get; }
    string Descripcion { get; }
    int CalcularPuntos(AccesoAtraccion acceso, IEnumerable<AccesoAtraccion> accesosDelDia);
}
