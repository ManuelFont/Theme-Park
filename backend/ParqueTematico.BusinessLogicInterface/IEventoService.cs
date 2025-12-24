using Dominio.Entities;

namespace ParqueTematico.BusinessLogicInterface;

public interface IEventoService
{
    Evento CrearEvento(string nombre, DateTime fecha, TimeSpan duracion, int aforo, decimal costo);

    void Eliminar(Guid id);

    IEnumerable<Evento> ObtenerTodos();

    Evento? ObtenerPorId(Guid id);

    void AgregarAtraccionAEvento(Guid eventoId, Guid atraccionId);

    void EliminarAtraccionDeEvento(Guid eventoId, Guid atraccionId);
}
