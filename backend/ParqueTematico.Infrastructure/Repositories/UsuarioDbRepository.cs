using Dominio.Entities.Usuarios;
using Dominio.Exceptions;
using Microsoft.EntityFrameworkCore;
using RepositoryInterfaces;

namespace Infrastructure.Repositories;

public class UsuarioDbRepository(ParqueDbContext context) : IUsuarioRepository
{
    private readonly ParqueDbContext _context = context;

    public Usuario Agregar(Usuario usuario)
    {
        var email = usuario.Email.Trim().ToLowerInvariant();

        if(_context.Usuarios.AsNoTracking().Any(u => u.Email == email))
        {
            throw new ConflictException("Ya existe un usuario con ese email");
        }

        _context.Usuarios.Add(usuario);
        _context.SaveChanges();
        return usuario;
    }

    public Usuario? ObtenerPorId(Guid id)
    {
        return _context.Usuarios.Find(id);
    }

    public IList<Usuario> ObtenerTodos()
    {
        return _context.Usuarios.AsNoTracking().ToList();
    }

    public void Actualizar(Usuario usuarioConCambios)
    {
        var usuarioPersistido = _context.Usuarios.FirstOrDefault(u => u.Id == usuarioConCambios.Id)
            ?? throw new NotFoundException("No se encontró un usuario con ese id");

        var emailNormalizado = usuarioConCambios.Email.Trim().ToLowerInvariant();

        var existeOtroConMismoEmail = _context.Usuarios.AsNoTracking()
            .Any(u => u.Id != usuarioConCambios.Id && u.Email == emailNormalizado);

        if(existeOtroConMismoEmail)
        {
            throw new ConflictException("Ya existe un usuario con ese email");
        }

        _context.Entry(usuarioPersistido).CurrentValues.SetValues(usuarioConCambios);
        var entradaContrasenia = _context.Entry(usuarioPersistido)
            .Reference(u => u.Contrasenia).TargetEntry;

        if(entradaContrasenia is not null)
        {
            entradaContrasenia.CurrentValues.SetValues(usuarioConCambios.Contrasenia);
        }

        if(usuarioPersistido is Visitante visitantePersistido &&
           usuarioConCambios is Visitante visitanteConCambios)
        {
            _context.Entry(visitantePersistido)
                .Property(nameof(Visitante.FechaNacimiento))
                .CurrentValue = visitanteConCambios.FechaNacimiento;

            _context.Entry(visitantePersistido)
                .Property(nameof(Visitante.NivelMembresia))
                .CurrentValue = visitanteConCambios.NivelMembresia;
        }

        _context.SaveChanges();
    }

    public void Eliminar(Guid id)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);
        if(usuario == null)
        {
            throw new NotFoundException("No se encontró un usuario con ese id");
        }

        _context.Usuarios.Remove(usuario);
        _context.SaveChanges();
    }
}
