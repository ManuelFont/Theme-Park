namespace Dtos;

public class UsuarioDTO(
    Guid id,
    string nombre,
    string apellido,
    string email,
    string tipoUsuario,
    DateTime? fechaNacimiento,
    string? nivelMembresia,
    int? puntosActuales
)
{
    public Guid Id { get; } = id;
    public string Nombre { get; } = nombre;
    public string Apellido { get; } = apellido;
    public string Email { get; } = email;
    public string TipoUsuario { get; } = tipoUsuario;
    public DateTime? FechaNacimiento { get; } = fechaNacimiento;
    public string? NivelMembresia { get; } = nivelMembresia;
    public int? PuntosActuales { get; } = puntosActuales;
}
