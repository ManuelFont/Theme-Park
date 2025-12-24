using Dominio.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ParqueTematico.WebApi.Filtros.ExceptionHandlers;

public class NotFoundExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception)
    {
        return exception is NotFoundException;
    }

    public IActionResult Handle(Exception exception)
    {
        return new NotFoundObjectResult(new
        {
            mensaje = exception.Message
        });
    }
}
