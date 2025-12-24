using Dominio.Exceptions;

namespace Dominio.Entities.Usuarios;

/// <summary>
///     Representa los distintos niveles de membresía de un visitante.
/// </summary>
public enum NivelMembresia
{
    /// <summary>
    ///     Membresía estándar, con acceso básico.
    /// </summary>
    Estandar,

    /// <summary>
    ///     Membresía premium, con beneficios adicionales.
    /// </summary>
    Premium,

    /// <summary>
    ///     Membresía VIP, con todos los beneficios disponibles.
    /// </summary>
    Vip
}

public class Visitante : Usuario
{
    private Visitante()
    {
    }

    public Visitante(
        string nombre,
        string apellido,
        string email,
        string contrasenia,
        DateTime fechaNacimiento,
        NivelMembresia nivelMembresia)
        : base(nombre, apellido, email, contrasenia)
    {
        if(fechaNacimiento > DateTime.UtcNow.Date)
        {
            throw new UserException("La fecha de nacimiento no puede ser futura");
        }

        FechaNacimiento = fechaNacimiento.Date;
        NivelMembresia = nivelMembresia;
    }

    public Guid NfcId { get; private set; } = Guid.NewGuid();
    public DateTime FechaNacimiento { get; set; }
    public NivelMembresia NivelMembresia { get; set; }
    public int PuntosActuales { get; private set; } = 0;

    public override string ToString()
    {
        return "Visitante";
    }

    public int EdadEn(DateTime fecha)
    {
        return (int)((fecha.Date - FechaNacimiento.Date).TotalDays / 365.2425);
    }

    public void CambiarMembresiaPorAdministracion(NivelMembresia nueva)
    {
        NivelMembresia = nueva;
    }

    public void AgregarPuntos(int puntos)
    {
        if(puntos < 0)
        {
            throw new UserException("No se pueden agregar puntos negativos");
        }

        PuntosActuales += puntos;
    }

    public void DescontarPuntos(int puntos)
    {
        if(puntos < 0)
        {
            throw new UserException("No se pueden descontar puntos negativos");
        }

        if(PuntosActuales < puntos)
        {
            throw new UserException("Puntos insuficientes para realizar esta operación");
        }

        PuntosActuales -= puntos;
    }
}
