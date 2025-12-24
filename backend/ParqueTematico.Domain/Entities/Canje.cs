using Dominio.Entities.Usuarios;

namespace Dominio.Entities;

public class Canje
{
    public Guid Id { get; } = Guid.NewGuid();
    public Usuario Usuario { get; set; } = null!;
    public Recompensa Recompensa { get; set; } = null!;
    public DateTime FechaCanje { get; set; }

    protected Canje()
    {
    }

    public Canje(Usuario usuario, Recompensa recompensa, DateTime fechaCanje)
    {
        Usuario = usuario;
        Recompensa = recompensa;
        FechaCanje = fechaCanje;
    }
}
