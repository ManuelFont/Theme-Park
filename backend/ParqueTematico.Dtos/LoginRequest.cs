namespace Dtos;

public class LoginRequest(string email, string contrasenia)
{
    public string Email { get; } = email;
    public string Contrasenia { get; } = contrasenia;
}
