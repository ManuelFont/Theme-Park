using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ParqueTematico.BusinessLogicInterface;

namespace ParqueTematico.WebApi.Filtros;

public sealed class TipoUsuarioFilter(string[] rolesRequeridos, IAuthService authService)
    : Attribute, IAuthorizationFilter
{
    private readonly IAuthService _authService = authService;
    private readonly string[] _rolesRequeridos = rolesRequeridos;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

        if(authHeader == null || !authHeader.StartsWith("Bearer "))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        var principal = _authService.ValidarToken(token);

        if(principal == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var role = principal.Claims.FirstOrDefault(c => c.Type == "tipoUsuario")?.Value;

        if(!_rolesRequeridos.Contains(role))
        {
            context.Result = new ForbidResult();
            return;
        }

        context.HttpContext.User = principal;
    }
}
