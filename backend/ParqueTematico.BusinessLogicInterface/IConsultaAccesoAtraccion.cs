using Dominio.Entities;
using Dtos;

namespace ParqueTematico.BusinessLogicInterface;

public interface IConsultaAccesoAtraccion
{
    AccesoAtraccion? ObtenerPorId(Guid id);

    IEnumerable<AccesoAtraccion> ObtenerAccesosEntreFechas(DateTime fechaInicio, DateTime fechaFin);

    IEnumerable<AccesoAtraccion> ObtenerAccesosPorVisitanteYFecha(Guid visitanteId, DateTime fecha);

    int ObtenerAforoActual(Guid atraccionId);

    List<ReporteUsoAtraccionDto> ObtenerReporteUsoAtracciones(DateTime fechaInicio, DateTime fechaFin);
}
