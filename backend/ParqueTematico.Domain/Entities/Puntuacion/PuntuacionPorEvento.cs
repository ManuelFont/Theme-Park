namespace Dominio.Entities.Puntuacion;

public class PuntuacionPorEvento(decimal multiplicador = 1.5m) : IPuntuacion
{
    private readonly decimal _multiplicador = multiplicador;

    public string Nombre => "Puntuación Por Evento";
    public string Descripcion => "Multiplica los puntos base cuando el visitante tiene un ticket de evento especial";

    public int CalcularPuntos(AccesoAtraccion acceso, IEnumerable<AccesoAtraccion> accesosDelDia)
    {
        var puntosBase = CalcularPuntosBase(acceso.Atraccion.Tipo);

        if(EsDuranteEvento(acceso))
        {
            return (int)(puntosBase * _multiplicador);
        }

        return puntosBase;
    }

    private bool EsDuranteEvento(AccesoAtraccion acceso)
    {
        return acceso.Ticket.TipoEntrada == TipoEntrada.EventoEspecial
               && acceso.Ticket.EventoAsociado != null;
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
