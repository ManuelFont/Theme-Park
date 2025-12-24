using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Dominio.Exceptions;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace ParqueTematico.Application.Services.Internal;

internal sealed class GestionPuntosAccesoService(
    IAccesoAtraccionRepository accesoRepo,
    IRankingService rankingService,
    IHistorialPuntuacionService historialServiceService,
    IUsuarioRepository usuarioRepo)
{
    private readonly IAccesoAtraccionRepository _accesoRepo = accesoRepo;
    private readonly IHistorialPuntuacionService _historialServiceService = historialServiceService;
    private readonly IRankingService _rankingService = rankingService;
    private readonly IUsuarioRepository _usuarioRepo = usuarioRepo;

    public void RegistrarHistorialPuntos(AccesoAtraccion acceso)
    {
        IEnumerable<AccesoAtraccion> todosLosAccesosDelDia = ObtenerAccesosDelDia(acceso.FechaHoraIngreso);
        var puntos = _rankingService.ObtenerEstrategiaActiva().CalcularPuntos(acceso, todosLosAccesosDelDia);
        var nombreEstrategia = _rankingService.ObtenerNombreEstrategiaActiva();

        var historial = new HistorialPuntuacion(acceso.Visitante, puntos, acceso.Atraccion.Nombre, nombreEstrategia,
            acceso.FechaHoraIngreso);

        _historialServiceService.RegistrarHistorial(historial);

        var visitante = (Visitante)acceso.Visitante;
        visitante.AgregarPuntos(puntos);
        _usuarioRepo.Actualizar(visitante);
    }

    private IEnumerable<AccesoAtraccion> ObtenerAccesosDelDia(DateTime fecha)
    {
        DateTime inicioDelDia = fecha.Date;
        DateTime finDelDia = fecha.Date.AddDays(1).AddTicks(-1);
        return _accesoRepo.ObtenerAccesosEntreFechas(inicioDelDia, finDelDia);
    }

    public void AsignarPuntos(Guid accesoId, int puntos)
    {
        AccesoAtraccion? acceso = _accesoRepo.ObtenerPorId(accesoId);
        if(acceso == null)
        {
            throw new AccesoAtraccionException("El acceso no existe");
        }

        acceso.AsignarPuntos(puntos);
        _accesoRepo.Actualizar(acceso);
    }
}
