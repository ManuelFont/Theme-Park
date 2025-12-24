using Dtos;

namespace ParqueTematico.BusinessLogicInterface;

public interface IRecompensaService
{
    RecompensaCreadoDto CrearRecompensa(RecompensaCrearDto recompensaCrearDto);
    IList<RecompensaCreadoDto> ObtenerTodos();
    RecompensaCreadoDto ObtenerRecompensa(Guid id);
    RecompensaCreadoDto ActualizarRecompensa(Guid id, RecompensaCrearDto recompensaCreadoDto);
    void EliminarRecompensa(Guid id);
}
