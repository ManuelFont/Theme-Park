namespace Dominio.Entities.Usuarios;

public class Operador : Usuario
{
    public Operador(string nombre, string apellido, string email, string contrasenia)
        : base(nombre, apellido, email, contrasenia)
    {
    }

    protected Operador()
    {
    }

    public override string ToString()
    {
        return "Operador";
    }
}
