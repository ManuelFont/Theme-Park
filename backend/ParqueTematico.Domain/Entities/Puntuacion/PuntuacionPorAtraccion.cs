namespace Dominio.Entities.Puntuacion;

public class PuntuacionPorAtraccion : IPuntuacion
{
    public string Nombre => "Puntuación Por Atracción";
    public string Descripcion => "Asigna puntos fijos según el tipo de atracción (Montaña Rusa: 100, Simulador: 75, Espectáculo: 50, Zona Interactiva: 25)";

    public int CalcularPuntos(AccesoAtraccion acceso, IEnumerable<AccesoAtraccion> accesosDelDia)
    {
        TipoAtraccion tipo = acceso.Atraccion.Tipo;

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
