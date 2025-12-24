using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Dtos;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace ParqueTematico.Application.Services;

public class CanjeService(
    ICanjeRepository repo,
    IUsuarioRepository usuarioRepo,
    IBaseRepository<Recompensa> recompensaRepo,
    IRelojService reloj) : ICanjeService
{
    private readonly IBaseRepository<Recompensa> _recompensaRepo = recompensaRepo;
    private readonly IRelojService _reloj = reloj;
    private readonly ICanjeRepository _repo = repo;
    private readonly IUsuarioRepository _usuarioRepo = usuarioRepo;

    public CanjeCreadoDto CrearCanje(CanjeCrearDto canjeCrearDto)
    {
        Usuario? posibleUsuario = _usuarioRepo.ObtenerPorId(canjeCrearDto.UsuarioId);
        Recompensa? posibleRecompensa = _recompensaRepo.ObtenerPorId(canjeCrearDto.RecompensaId);

        ValidarVisitanteRecompensaExisten(posibleUsuario, posibleRecompensa);
        var visitante = (Visitante)posibleUsuario!;
        Recompensa recompensa = posibleRecompensa!;

        ValidarPuntosMembresiaCantidad(visitante, recompensa);

        visitante.DescontarPuntos(recompensa.Costo);
        recompensa.CantidadDisponible--;

        _usuarioRepo.Actualizar(visitante);

        var canje = new Canje(visitante, recompensa, _reloj.ObtenerFechaHora());
        _repo.Agregar(canje);
        return new CanjeCreadoDto
        {
            Id = canje.Id,
            UsuarioId = canje.Usuario.Id,
            RecompensaId = canje.Recompensa.Id,
            FechaCanje = canje.FechaCanje
        };
    }

    public IList<CanjeCreadoDto> ObtenerTodos()
    {
        IList<CanjeCreadoDto> dtos = [];
        foreach(Canje canje in _repo.ObtenerTodos())
        {
            var dto = new CanjeCreadoDto
            {
                Id = canje.Id,
                UsuarioId = canje.Usuario.Id,
                RecompensaId = canje.Recompensa.Id,
                FechaCanje = canje.FechaCanje
            };
            dtos.Add(dto);
        }

        return dtos;
    }

    public IList<CanjeCreadoDto> ObtenerPorVisitante(Guid visitanteId)
    {
        IList<CanjeCreadoDto> dtos = [];
        foreach(Canje canje in _repo.ObtenerPorVisitante(visitanteId))
        {
            var dto = new CanjeCreadoDto
            {
                Id = canje.Id,
                UsuarioId = canje.Usuario.Id,
                RecompensaId = canje.Recompensa.Id,
                RecompensaNombre = canje.Recompensa.Nombre,
                PuntosCanjeados = canje.Recompensa.Costo,
                FechaCanje = canje.FechaCanje
            };
            dtos.Add(dto);
        }

        return dtos;
    }

    public CanjeCreadoDto ObtenerCanje(Guid id)
    {
        Canje? canje = _repo.ObtenerPorId(id);
        if(canje != null)
        {
            return new CanjeCreadoDto
            {
                Id = canje.Id,
                UsuarioId = canje.Usuario.Id,
                RecompensaId = canje.Recompensa.Id,
                FechaCanje = canje.FechaCanje
            };
        }

        throw new InvalidOperationException("No existe un Canje con ese Id");
    }

    public CanjeCreadoDto ActualizarCanje(Guid id, CanjeCrearDto canjeCrearDto)
    {
        Canje canjeDb = _repo.ObtenerPorId(id) ??
                        throw new InvalidOperationException("No existe un Canje con ese Id");
        canjeDb.Usuario = _usuarioRepo.ObtenerPorId(canjeCrearDto.UsuarioId) ??
                          throw new InvalidOperationException("No existe usuario con ese Id");
        canjeDb.Recompensa = _recompensaRepo.ObtenerPorId(canjeCrearDto.RecompensaId) ??
                             throw new InvalidOperationException("No existe recompensa con ese Id");
        _repo.Actualizar(canjeDb);

        return new CanjeCreadoDto
        {
            Id = canjeDb.Id,
            UsuarioId = canjeDb.Usuario.Id,
            RecompensaId = canjeDb.Recompensa.Id,
            FechaCanje = canjeDb.FechaCanje
        };
    }

    public void EliminarCanje(Guid id)
    {
        if(_repo.ObtenerPorId(id) == null)
        {
            throw new InvalidOperationException("No existe Canje con ese Id");
        }

        _repo.Eliminar(id);
    }

    private void ValidarVisitanteRecompensaExisten(Usuario? usuario, Recompensa? recompensa)
    {
        if(usuario == null)
        {
            throw new InvalidOperationException("No existe un usuario con ese Id");
        }

        if(usuario is not Visitante)
        {
            throw new InvalidOperationException("Solo los visitantes pueden canjear recompensas");
        }

        if(recompensa == null)
        {
            throw new InvalidOperationException("No existe una recompensa con ese Id");
        }
    }

    private void ValidarPuntosMembresiaCantidad(Visitante visitante, Recompensa recompensa)
    {
        if(visitante.PuntosActuales < recompensa.Costo)
        {
            throw new InvalidOperationException("Puntos insuficientes para canjear esta recompensa");
        }

        if(recompensa.CantidadDisponible <= 0)
        {
            throw new InvalidOperationException("Esta recompensa tiene cantidad insuficiente");
        }

        if(recompensa.NivelRequerido.HasValue)
        {
            if(visitante.NivelMembresia < recompensa.NivelRequerido.Value)
            {
                throw new InvalidOperationException("El usuario no tiene la membresia necesaria");
            }
        }
    }
}
