using Dominio.Entities;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace ParqueTematico.Application.Services;

public class HistorialPuntuacionService(IHistorialPuntuacionRepository repository) : IHistorialPuntuacionService
{
    private readonly IHistorialPuntuacionRepository _repository = repository;

    public IList<HistorialPuntuacion> ObtenerHistorialPorVisitante(Guid visitanteId)
    {
        return _repository.ObtenerPorVisitante(visitanteId);
    }

    public void RegistrarHistorial(HistorialPuntuacion historial)
    {
        _repository.Agregar(historial);
    }
}
