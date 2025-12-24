namespace Dominio.Entities;

public class Reloj
{
    public Reloj()
    {
        FechaHora = DateTime.Now;
    }

    public int Id { get; set; }
    public DateTime FechaHora { get; set; }
}
