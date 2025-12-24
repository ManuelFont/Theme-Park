using Microsoft.AspNetCore.Mvc;

namespace ParqueTematico.WebApi.Filtros;

public interface IExceptionHandler
{
    bool CanHandle(Exception exception);

    IActionResult Handle(Exception exception);
}
