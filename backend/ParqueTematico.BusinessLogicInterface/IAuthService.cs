using System.Security.Claims;

namespace ParqueTematico.BusinessLogicInterface;

public interface IAuthService
{
    string GenerarToken(string email);

    bool ValidarUsuario(string email, string contrasenia);

    ClaimsPrincipal? ValidarToken(string token);

    Guid ObtenerUserIdDeClaims(ClaimsPrincipal user);
}
