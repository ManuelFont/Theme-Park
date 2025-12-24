using Dominio.Entities.Usuarios;
using Dominio.Exceptions;
using Dominio.ValueObjects;
using Dtos;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace ParqueTematico.Application.Services;

public class UsuarioService(IUsuarioRepository repository) : IUsuarioService
{
    private readonly IUsuarioRepository _repository = repository;

    public void InicializarAdministrador()
    {
        if(!ExisteAdministrador())
        {
            var request = new CrearUsuarioRequest
            {
                Nombre = "admin",
                Apellido = "admin",
                Email = "admin@admin.com",
                Contrasenia = "Admin123456798!",
                TipoUsuario = "Administrador",
            };
            CrearUsuario(request);
        }
    }

    public UsuarioDTO CrearUsuario(CrearUsuarioRequest request)
    {
        Usuario usuario = request.TipoUsuario.ToLowerInvariant() switch
        {
            "administrador" => new Administrador(
                request.Nombre,
                request.Apellido,
                request.Email,
                request.Contrasenia),
            "operador" => new Operador(
                request.Nombre,
                request.Apellido,
                request.Email,
                request.Contrasenia),
            "visitante" => new Visitante(
                request.Nombre,
                request.Apellido,
                request.Email,
                request.Contrasenia,
                request.FechaNacimiento ?? throw new UserException("Fecha de nacimiento requerida"),
                request.NivelMembresia ?? throw new UserException("Nivel de membresía requerido")),
            _ => throw new UserException("Tipo de usuario inválido"),
        };

        _repository.Agregar(usuario);
        return new UsuarioDTO(
            usuario.Id,
            usuario.Nombre,
            usuario.Apellido,
            usuario.Email,
            request.TipoUsuario,
            (usuario as Visitante)?.FechaNacimiento,
            (usuario as Visitante)?.NivelMembresia.ToString(),
            (usuario as Visitante)?.PuntosActuales);
    }

    public IList<UsuarioDTO> ListarUsuarios()
    {
        var usuarios = _repository.ObtenerTodos();
        var dtos = new List<UsuarioDTO>();

        foreach(var usuario in usuarios)
        {
            dtos.Add(
                new UsuarioDTO(
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Apellido,
                    usuario.Email,
                    usuario.ToString(),
                    (usuario as Visitante)?.FechaNacimiento,
                    (usuario as Visitante)?.NivelMembresia.ToString(),
                    (usuario as Visitante)?.PuntosActuales));
        }

        return dtos;
    }

    public string ObtenerTipoPorEmail(string email)
    {
        return ObtenerUsuarioPorEmail(email).ToString();
    }

    public Guid ObtenerIdPorEmail(string email)
    {
        return ObtenerUsuarioPorEmail(email).Id;
    }

    private Usuario ObtenerUsuarioPorEmail(string email)
    {
        var usuario = _repository.ObtenerTodos().FirstOrDefault(u => u.Email == email);

        if(usuario == null)
        {
            throw new KeyNotFoundException($"No se encontró el usuario con email '{email}'");
        }

        return usuario;
    }

    private bool ExisteAdministrador()
    {
        foreach(UsuarioDTO u in ListarUsuarios())
        {
            if(u.TipoUsuario == "Administrador")
            {
                return true;
            }
        }

        return false;
    }

    public UsuarioDTO? ObtenerPorId(Guid id)
    {
        var usuario = _repository.ObtenerPorId(id);
        return usuario == null
            ? null
            : new UsuarioDTO(
                usuario.Id,
                usuario.Nombre,
                usuario.Apellido,
                usuario.Email,
                usuario.ToString(),
                (usuario as Visitante)?.FechaNacimiento,
                (usuario as Visitante)?.NivelMembresia.ToString(),
                (usuario as Visitante)?.PuntosActuales);
    }

    public void Eliminar(Guid id)
    {
        _repository.Eliminar(id);
    }

    public void Actualizar(Guid id, ActualizarUsuarioRequest request)
    {
        var usuario = _repository.ObtenerPorId(id);
        if(usuario == null)
        {
            throw new KeyNotFoundException();
        }

        usuario.Nombre = request.Nombre;
        usuario.Apellido = request.Apellido;
        usuario.Email = request.Email;
        if(!string.IsNullOrWhiteSpace(request.Contrasenia))
        {
            usuario.Contrasenia = new Contrasenia(request.Contrasenia);
        }

        if(usuario is Visitante visitante)
        {
            if(request.FechaNacimiento == null || request.NivelMembresia == null)
            {
                throw new UserException(
                    "Fecha de nacimiento y nivel de membresía son requeridos para visitantes.");
            }

            visitante.FechaNacimiento = request.FechaNacimiento.Value;
            visitante.NivelMembresia = request.NivelMembresia.Value;
        }

        _repository.Actualizar(usuario);
    }

    public bool ValidarEmailContrasenia(string email, string contrasenia)
    {
        var usuarios = _repository.ObtenerTodos();
        var contraseniaHash = new Contrasenia(contrasenia);

        return usuarios.Any(u => u.Email == email && u.Contrasenia.Hash == contraseniaHash.Hash);
    }
}
