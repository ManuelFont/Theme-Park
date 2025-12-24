using Microsoft.AspNetCore.Mvc.Filters;

namespace ParqueTematico.WebApi.Filtros;

public class ExceptionFilter(IEnumerable<IExceptionHandler> handlers) : IExceptionFilter
{
    private readonly IEnumerable<IExceptionHandler> _handlers = handlers;

    public void OnException(ExceptionContext context)
    {
        foreach(IExceptionHandler handler in _handlers)
        {
            if(handler.CanHandle(context.Exception))
            {
                context.Result = handler.Handle(context.Exception);
                context.ExceptionHandled = true;
                return;
            }
        }
    }
}
