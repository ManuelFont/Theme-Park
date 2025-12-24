using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ParqueTematico.WebApi.Filtros.ExceptionHandlers;

public class ValidationExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception)
    {
        return exception is ValidationException;
    }

    public IActionResult Handle(Exception exception)
    {
        return new BadRequestObjectResult(new
        {
            mensaje = exception.Message
        });
    }
}
