using Microsoft.AspNetCore.Mvc;

namespace ParqueTematico.WebApi.Filtros.ExceptionHandlers;

public class DefaultExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception)
    {
        return true;
    }

    public IActionResult Handle(Exception exception)
    {
        return new ObjectResult(new
        {
            mensaje = "Error interno del servidor"
        })
        {
            StatusCode = 500
        };
    }
}
