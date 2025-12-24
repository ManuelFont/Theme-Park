namespace Dominio.Entities.Puntuacion;

public class PuntuacionCombo(int minutosMaximos = 30, int bonificacion = 50) : IPuntuacion
{
    private readonly int _bonificacion = bonificacion;
    private readonly int _minutosMaximos = minutosMaximos;

    public string Nombre => "Puntuación Combo";
    public string Descripcion => "Otorga puntos extra si visitas otra atracción de diferente tipo en menos de 30 minutos";

    public int CalcularPuntos(AccesoAtraccion acceso, IEnumerable<AccesoAtraccion> accesosDelDia)
    {
        var puntosBase = CalcularPuntosBase(acceso.Atraccion.Tipo);

        if(FormaCombo(acceso, accesosDelDia))
        {
            return puntosBase + _bonificacion;
        }

        return puntosBase;
    }

    private bool FormaCombo(AccesoAtraccion accesoActual, IEnumerable<AccesoAtraccion> accesosDelDia)
    {
        var accesosPrevios = accesosDelDia
            .Where(a => a.FechaHoraIngreso < accesoActual.FechaHoraIngreso
                        && a.FechaHoraEgreso.HasValue
                        && a.Atraccion.Tipo != accesoActual.Atraccion.Tipo)
            .OrderByDescending(a => a.FechaHoraEgreso)
            .ToList();

        if(!accesosPrevios.Any())
        {
            return false;
        }

        AccesoAtraccion accesoAnterior = accesosPrevios.First();
        TimeSpan diferencia = accesoActual.FechaHoraIngreso - accesoAnterior.FechaHoraEgreso!.Value;

        return diferencia.TotalMinutes <= _minutosMaximos;
    }

    private int CalcularPuntosBase(TipoAtraccion tipo)
    {
        if(tipo == TipoAtraccion.MontañaRusa)
        {
            return 100;
        }

        if(tipo == TipoAtraccion.Simulador)
        {
            return 75;
        }

        if(tipo == TipoAtraccion.Espectáculo)
        {
            return 50;
        }

        if(tipo == TipoAtraccion.ZonaInteractiva)
        {
            return 25;
        }

        return 0;
    }
}
