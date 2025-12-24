using Dominio.Entities;

namespace RepositoryInterfaces;

public interface IAccesoAtraccionRepository : IBaseRepository<AccesoAtraccion>
{
    IList<AccesoAtraccion> ObtenerAccesosSinEgresoPorAtraccion(Guid atraccionId);
    IList<AccesoAtraccion> ObtenerAccesosPorVisitanteYFecha(Guid visitanteId, DateTime fecha);
    IList<AccesoAtraccion> ObtenerAccesosEntreFechas(DateTime fechaInicio, DateTime fechaFin);
}
