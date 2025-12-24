using Microsoft.AspNetCore.Mvc;

namespace ParqueTematico.WebApi.Filtros.ExceptionHandlers;

public class InvalidOperationExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception)
    {
        return exception is InvalidOperationException;
    }

    public IActionResult Handle(Exception exception)
    {
        return new NotFoundObjectResult(new
        {
            mensaje = exception.Message
        });
    }
}
