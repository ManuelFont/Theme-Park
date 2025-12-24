using Microsoft.AspNetCore.Mvc;

namespace ParqueTematico.WebApi.Filtros;
public class RequiereRolAttribute : TypeFilterAttribute
{
    public RequiereRolAttribute(params string[] rolesRequeridos)
        : base(typeof(TipoUsuarioFilter))
    {
        Arguments = [rolesRequeridos];
    }
}
