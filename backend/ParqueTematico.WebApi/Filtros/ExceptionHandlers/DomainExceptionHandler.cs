using Dominio.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ParqueTematico.WebApi.Filtros.ExceptionHandlers;

public class DomainExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception)
    {
        return exception is DomainException;
    }

    public IActionResult Handle(Exception exception)
    {
        return new BadRequestObjectResult(new
        {
            mensaje = exception.Message
        });
    }
}
