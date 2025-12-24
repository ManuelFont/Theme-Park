using Dominio.Entities;
using Dtos;

namespace ParqueTematico.BusinessLogicInterface;

public interface IAccesoAtraccionService
{
    Guid RegistrarIngreso(Guid ticketId, Guid atraccionId);

    void RegistrarEgreso(Guid accesoId);

    void AsignarPuntos(Guid accesoId, int puntos);

    IEnumerable<AccesoAtraccion> ObtenerAccesosEntreFechas(DateTime fechaInicio, DateTime fechaFin);

    int ObtenerAforoActual(Guid atraccionId);

    AccesoAtraccion? ObtenerPorId(Guid id);

    IEnumerable<AccesoAtraccion> ObtenerAccesosPorVisitanteYFecha(Guid visitanteId, DateTime fecha);

    List<ReporteUsoAtraccionDto> ObtenerReporteUsoAtracciones(DateTime fechaInicio, DateTime fechaFin);
}
