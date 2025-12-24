using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Dominio.Exceptions;

namespace Dominio.ValueObjects;

public class Contrasenia
{
    private Contrasenia()
    {
        Hash = string.Empty;
    }

    public Contrasenia(string valorPlano)
    {
        if(string.IsNullOrWhiteSpace(valorPlano))
        {
            throw new UserException("La contraseña no puede estar vacía");
        }

        if(valorPlano.Length < 8)
        {
            throw new UserException("La contraseña debe tener al menos 8 caracteres");
        }

        if(!Regex.IsMatch(valorPlano, @"[A-Z]"))
        {
            throw new UserException("Debe tener mayúscula");
        }

        if(!Regex.IsMatch(valorPlano, @"[a-z]"))
        {
            throw new UserException("Debe tener minúscula");
        }

        if(!Regex.IsMatch(valorPlano, @"\d"))
        {
            throw new UserException("Debe tener número");
        }

        if(!Regex.IsMatch(valorPlano, @"[^a-zA-Z0-9]"))
        {
            throw new UserException("Debe tener carácter especial");
        }

        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(valorPlano);
        Hash = Convert.ToBase64String(sha.ComputeHash(bytes));
    }

    public string Hash { get; }

    public bool Validar(string intento)
    {
        using var sha = SHA256.Create();
        var hashIntento = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(intento)));
        return Hash == hashIntento;
    }

    public override string ToString()
    {
        return "********";
    }
}
