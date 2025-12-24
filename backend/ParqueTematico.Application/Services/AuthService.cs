using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ParqueTematico.BusinessLogicInterface;

namespace ParqueTematico.Application.Services;

public class AuthService(IConfiguration configuration, IUsuarioService usuarioService) : IAuthService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IUsuarioService _usuarioService = usuarioService;

    public string GenerarToken(string email)
    {
        var secret = _configuration["JwtSettings:SecretKey"];
        if(string.IsNullOrEmpty(secret))
        {
            throw new NullReferenceException("No se encontró la clave secreta");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim("email", email),
                new Claim("tipoUsuario", _usuarioService.ObtenerTipoPorEmail(email)),
                new Claim(ClaimTypes.Name, email),
                new Claim("id", _usuarioService.ObtenerIdPorEmail(email).ToString())
            ]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = creds
        };

        SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public bool ValidarUsuario(string email, string contrasenia)
    {
        return _usuarioService.ValidarEmailContrasenia(email, contrasenia);
    }

    public ClaimsPrincipal? ValidarToken(string token)
    {
        var secret = _configuration["JwtSettings:SecretKey"];
        if(string.IsNullOrEmpty(secret))
        {
            throw new NullReferenceException("No se encontró la clave secreta");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            ClaimsPrincipal? principal = tokenHandler.ValidateToken(token, parameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    public Guid ObtenerUserIdDeClaims(ClaimsPrincipal user)
    {
        var idClaim = user.Claims.FirstOrDefault(c => c.Type == "id");
        if(idClaim == null)
        {
            throw new UnauthorizedAccessException("No se encontró el claim de ID de usuario");
        }

        return Guid.Parse(idClaim.Value);
    }
}
