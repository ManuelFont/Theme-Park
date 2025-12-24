using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ParqueTematico.BusinessLogicInterface;

namespace ParqueTematico.WebApi.Filtros;

public sealed class AuthorizationFilterAttribute(IAuthService service) : Attribute, IAuthorizationFilter
{
    private readonly IAuthService _service = service;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if(authHeader != null && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var principal = _service.ValidarToken(token);

            if(principal == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            context.HttpContext.User = principal;
        }
        else
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
