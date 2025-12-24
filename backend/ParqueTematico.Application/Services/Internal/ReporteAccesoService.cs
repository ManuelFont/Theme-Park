using Dominio.Entities;
using Dtos;
using RepositoryInterfaces;

namespace ParqueTematico.Application.Services.Internal;

internal sealed class ReporteAccesoService(IAccesoAtraccionRepository accesoRepo)
{
    private readonly IAccesoAtraccionRepository _accesoRepo = accesoRepo;

    public List<ReporteUsoAtraccionDto> ObtenerReporteUsoAtracciones(DateTime fechaInicio, DateTime fechaFin)
    {
        var todosLosAccesos = _accesoRepo.ObtenerAccesosEntreFechas(fechaInicio, fechaFin).ToList();
        var reporte = new List<ReporteUsoAtraccionDto>();
        var atraccionesYaProcesadas = new List<Guid>();

        foreach(AccesoAtraccion acceso in todosLosAccesos)
        {
            Guid atraccionId = acceso.Atraccion.Id;

            if(!YaFueProcesada(atraccionesYaProcesadas, atraccionId))
            {
                var cantidadVisitas = ContarVisitasDeAtraccion(todosLosAccesos, atraccionId);

                reporte.Add(new ReporteUsoAtraccionDto
                {
                    AtraccionId = atraccionId,
                    NombreAtraccion = acceso.Atraccion.Nombre,
                    CantidadVisitas = cantidadVisitas
                });

                atraccionesYaProcesadas.Add(atraccionId);
            }
        }

        OrdenarReportePorCantidadVisitas(reporte);

        return reporte;
    }

    private bool YaFueProcesada(List<Guid> atraccionesYaProcesadas, Guid atraccionId)
    {
        foreach(Guid id in atraccionesYaProcesadas)
        {
            if(id == atraccionId)
            {
                return true;
            }
        }

        return false;
    }

    private int ContarVisitasDeAtraccion(List<AccesoAtraccion> todosLosAccesos, Guid atraccionId)
    {
        var contador = 0;
        foreach(AccesoAtraccion acceso in todosLosAccesos)
        {
            if(acceso.Atraccion.Id == atraccionId)
            {
                contador++;
            }
        }

        return contador;
    }

    private void OrdenarReportePorCantidadVisitas(List<ReporteUsoAtraccionDto> reporte)
    {
        for(var i = 0; i < reporte.Count - 1; i++)
        {
            for(var j = 0; j < reporte.Count - i - 1; j++)
            {
                if(reporte[j].CantidadVisitas < reporte[j + 1].CantidadVisitas)
                {
                    ReporteUsoAtraccionDto temp = reporte[j];
                    reporte[j] = reporte[j + 1];
                    reporte[j + 1] = temp;
                }
            }
        }
    }
}
