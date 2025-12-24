using Dtos;

namespace ParqueTematico.BusinessLogicInterface;

public interface IUsuarioService
{
    void InicializarAdministrador();

    UsuarioDTO CrearUsuario(CrearUsuarioRequest request);

    IList<UsuarioDTO> ListarUsuarios();

    string ObtenerTipoPorEmail(string email);

    Guid ObtenerIdPorEmail(string email);

    UsuarioDTO? ObtenerPorId(Guid id);

    void Eliminar(Guid id);

    void Actualizar(Guid id, ActualizarUsuarioRequest request);

    bool ValidarEmailContrasenia(string email, string contrasenia);
}
