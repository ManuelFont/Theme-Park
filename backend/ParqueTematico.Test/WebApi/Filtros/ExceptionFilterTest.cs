using System.Net;
using Dominio.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using ParqueTematico.WebApi.Filtros;
using ParqueTematico.WebApi.Filtros.ExceptionHandlers;
using RouteData = Microsoft.AspNetCore.Routing.RouteData;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace Test.WebApi.Filtros;

[TestClass]
public class ExceptionFilterTest
{
    private ExceptionFilter _filter = null!;

    [TestInitialize]
    public void Setup()
    {
        var handlers = new List<IExceptionHandler>
        {
            new NotFoundExceptionHandler(),
            new ConflictExceptionHandler(),
            new ValidationExceptionHandler(),
            new DomainExceptionHandler(),
            new InvalidOperationExceptionHandler(),
            new DefaultExceptionHandler()
        };
        _filter = new ExceptionFilter(handlers);
    }

    private ExceptionContext CreateExceptionContext(Exception exception)
    {
        var actionContext = new ActionContext
        {
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(),
            ActionDescriptor = new ActionDescriptor()
        };

        return new ExceptionContext(actionContext, []) { Exception = exception };
    }

    [TestMethod]
    public void OnException_InvalidOperationException_Returns404()
    {
        var exception = new InvalidOperationException("Invalid operation");
        ExceptionContext context = CreateExceptionContext(exception);

        _filter.OnException(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public void OnException_UnknownException_Returns500()
    {
        var exception = new Exception("Unexpected error");
        ExceptionContext context = CreateExceptionContext(exception);

        _filter.OnException(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, result.StatusCode);
    }

    [TestMethod]
    public void OnException_NotFoundException_Returns404()
    {
        var exception = new NotFoundException("No encontrado");
        ExceptionContext context = CreateExceptionContext(exception);

        _filter.OnException(context);

        var result = context.Result as NotFoundObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public void OnException_ConflictException_Returns409()
    {
        var exception = new ConflictException("Conflicto detectado");
        ExceptionContext context = CreateExceptionContext(exception);

        _filter.OnException(context);

        var result = context.Result as ConflictObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.Conflict, result.StatusCode);
    }

    [TestMethod]
    public void OnException_ValidationException_Returns400()
    {
        var exception = new ValidationException("Error de validacion");
        ExceptionContext context = CreateExceptionContext(exception);

        _filter.OnException(context);

        var result = context.Result as BadRequestObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
    }

    [TestMethod]
    public void OnException_DomainException_Returns400()
    {
        DomainException exception = new AtraccionException("Error de Dominio");
        ExceptionContext context = CreateExceptionContext(exception);

        _filter.OnException(context);

        var result = context.Result as BadRequestObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
    }
}
