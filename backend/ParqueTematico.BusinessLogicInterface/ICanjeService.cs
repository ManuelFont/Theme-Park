using Dtos;

namespace ParqueTematico.BusinessLogicInterface;

public interface ICanjeService
{
    CanjeCreadoDto CrearCanje(CanjeCrearDto canjeCrearDto);
    IList<CanjeCreadoDto> ObtenerTodos();
    IList<CanjeCreadoDto> ObtenerPorVisitante(Guid visitanteId);
    CanjeCreadoDto ObtenerCanje(Guid id);
    CanjeCreadoDto ActualizarCanje(Guid id, CanjeCrearDto canjeCrearDto);
    void EliminarCanje(Guid id);
}
