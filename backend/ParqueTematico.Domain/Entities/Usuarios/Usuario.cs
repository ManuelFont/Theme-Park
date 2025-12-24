using System.Text.RegularExpressions;
using Dominio.Exceptions;
using Dominio.ValueObjects;

namespace Dominio.Entities.Usuarios;

public abstract class Usuario
{
    protected Usuario()
    {
    }

    protected Usuario(string nombre, string apellido, string email, string contrasenia)
    {
        if(string.IsNullOrWhiteSpace(nombre))
        {
            throw new UserException("Nombre vacío");
        }

        if(string.IsNullOrWhiteSpace(apellido))
        {
            throw new UserException("Apellido vacío");
        }

        if(string.IsNullOrWhiteSpace(email))
        {
            throw new UserException("Email vacío");
        }

        nombre = nombre.Trim();
        apellido = apellido.Trim();
        email = email.Trim();

        if(nombre.Length > 50)
        {
            throw new UserException("Nombre muy largo");
        }

        if(apellido.Length > 50)
        {
            throw new UserException("Apellido muy largo");
        }

        if(!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new UserException("Email inválido");
        }

        Nombre = nombre;
        Apellido = apellido;
        Email = email.ToLowerInvariant();
        Contrasenia = new Contrasenia(contrasenia);
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Contrasenia Contrasenia { get; set; } = null!;

    public override string ToString()
    {
        return "Usuario sin Tipo";
    }

    public bool ValidarContrasenia(string intento)
    {
        return Contrasenia.Validar(intento);
    }
}
