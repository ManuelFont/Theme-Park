using Dominio.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ParqueTematico.WebApi.Filtros.ExceptionHandlers;

public class ConflictExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception)
    {
        return exception is ConflictException;
    }

    public IActionResult Handle(Exception exception)
    {
        return new ConflictObjectResult(new
        {
            mensaje = exception.Message
        });
    }
}
