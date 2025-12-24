using Dominio.Entities.Usuarios;

namespace Dtos;

public class ActualizarUsuarioRequest
{
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Contrasenia { get; set; } = null!;
    public DateTime? FechaNacimiento { get; set; }
    public NivelMembresia? NivelMembresia { get; set; }
}
