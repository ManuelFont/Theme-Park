using System.Text.Json.Serialization;
using Infrastructure;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using ParqueTematico.Application.Plugins;
using ParqueTematico.WebApi.Converters;
using ParqueTematico.WebApi.Filtros;
using ParqueTematico.WebApi.Filtros.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ParqueDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ParqueDb")));

builder.Services.AddCorsPolicy();
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddPluginSystem();

builder.Services.AddSingleton<IExceptionHandler, NotFoundExceptionHandler>();
builder.Services.AddSingleton<IExceptionHandler, ConflictExceptionHandler>();
builder.Services.AddSingleton<IExceptionHandler, ValidationExceptionHandler>();
builder.Services.AddSingleton<IExceptionHandler, DomainExceptionHandler>();
builder.Services.AddSingleton<IExceptionHandler, InvalidOperationExceptionHandler>();
builder.Services.AddSingleton<IExceptionHandler, DefaultExceptionHandler>();

builder
    .Services.AddControllers(options =>
    {
        options.Filters.Add<ExceptionFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new ParqueDateTimeConverter());
        options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(allowIntegerValues: false));
    });

var app = builder.Build();

var pluginsPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
app.Services.InicializarAplicacion(pluginsPath);

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
