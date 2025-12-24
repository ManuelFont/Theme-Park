using Dtos;
using Microsoft.AspNetCore.Mvc;
using ParqueTematico.BusinessLogicInterface;

namespace ParqueTematico.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService service) : ControllerBase
{
    private readonly IAuthService _service = service;

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        Console.WriteLine("entra");
        if(!_service.ValidarUsuario(request.Email, request.Contrasenia))
        {
            return Unauthorized(new { mensaje = "Credenciales inválidas" });
        }

        var token = _service.GenerarToken(request.Email);
        return Ok(new LoginResponse(token, DateTime.UtcNow.AddHours(1)));
    }
}
