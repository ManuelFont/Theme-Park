namespace Dominio.Entities.Usuarios;

public class Administrador : Usuario
{
    public Administrador(string nombre, string apellido, string email, string contrasenia)
        : base(nombre, apellido, email, contrasenia)
    {
    }

    protected Administrador()
    {
    }

    public override string ToString()
    {
        return "Administrador";
    }
}
