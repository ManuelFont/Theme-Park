using Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParqueTematico.BusinessLogicInterface;
using ParqueTematico.WebApi.Filtros;

namespace ParqueTematico.WebApi.Controllers;

[ApiController]
[Route("api/usuarios")]
public class UsuarioController(IUsuarioService service, IAuthService authService) : ControllerBase
{
    private readonly IUsuarioService _service = service;
    private readonly IAuthService _authService = authService;

    [HttpPost]
    [AllowAnonymous]
    public IActionResult CrearUsuario([FromBody] CrearUsuarioRequest request)
    {
        var usuario = _service.CrearUsuario(request);
        return CreatedAtAction(nameof(ObtenerUsuarioPorId), new { id = usuario.Id }, usuario);
    }

    [HttpGet]
    public IActionResult ListarUsuarios()
    {
        var usuarios = _service.ListarUsuarios();
        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    public IActionResult ObtenerUsuarioPorId(Guid id)
    {
        var usuario = _service.ObtenerPorId(id);

        if(usuario is null)
        {
            return NotFound(new { mensaje = "Usuario no encontrado" });
        }

        return Ok(usuario);
    }

    [HttpGet("mi-informacion")]
    [RequiereRol("Visitante")]
    public IActionResult ObtenerMiInformacion()
    {
        var userId = _authService.ObtenerUserIdDeClaims(User);
        var usuario = _service.ObtenerPorId(userId);

        if(usuario is null)
        {
            return NotFound(new { mensaje = "Usuario no encontrado" });
        }

        return Ok(usuario);
    }

    [HttpPut("{id}")]
    public IActionResult Actualizar(Guid id, [FromBody] ActualizarUsuarioRequest request)
    {
        _service.Actualizar(id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(Guid id)
    {
        _service.Eliminar(id);
        return NoContent();
    }
}
