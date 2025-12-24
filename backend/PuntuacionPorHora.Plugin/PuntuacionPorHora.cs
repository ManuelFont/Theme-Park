using Dominio.Entities;
using Dominio.Entities.Puntuacion;

namespace PuntuacionPorHora;

public class EstrategiaPuntuacionPorHora : IPuntuacion
{
    private readonly int _horaInicio;
    private readonly int _horaFin;
    private readonly int _multiplicador;

    public EstrategiaPuntuacionPorHora()
        : this(18, 22, 2)
    {
    }

    public EstrategiaPuntuacionPorHora(int horaInicio, int horaFin, int multiplicador)
    {
        _horaInicio = horaInicio;
        _horaFin = horaFin;
        _multiplicador = multiplicador;
    }

    public string Nombre => "Puntuación Por Hora";

    public string Descripcion =>
        $"Multiplica puntos por {_multiplicador} durante el horario {_horaInicio}:00 - {_horaFin}:00";

    public int CalcularPuntos(AccesoAtraccion acceso, IEnumerable<AccesoAtraccion> accesosDelDia)
    {
        var puntosBase = CalcularPuntosBase(acceso.Atraccion.Tipo);

        if (EstaEnHorarioBonus(acceso.FechaHoraIngreso))
        {
            return puntosBase * _multiplicador;
        }

        return puntosBase;
    }

    private bool EstaEnHorarioBonus(DateTime fechaHora)
    {
        int hora = fechaHora.Hour;
        return hora >= _horaInicio && hora < _horaFin;
    }

    private int CalcularPuntosBase(TipoAtraccion tipo)
    {
        if (tipo == TipoAtraccion.MontañaRusa)
        {
            return 100;
        }

        if (tipo == TipoAtraccion.Simulador)
        {
            return 75;
        }

        if (tipo == TipoAtraccion.Espectáculo)
        {
            return 50;
        }

        if (tipo == TipoAtraccion.ZonaInteractiva)
        {
            return 25;
        }

        return 0;
    }
}
