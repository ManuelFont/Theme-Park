using Dominio.Entities;
using Dominio.Exceptions;
using Dtos;
using ParqueTematico.BusinessLogicInterface;
using RepositoryInterfaces;

namespace ParqueTematico.Application.Services;

public class RecompensaService(IBaseRepository<Recompensa> repo) : IRecompensaService
{
    private readonly IBaseRepository<Recompensa> _repo = repo;

    public RecompensaCreadoDto CrearRecompensa(RecompensaCrearDto recompensaCrearDto)
    {
        var recompensa = new Recompensa(recompensaCrearDto.Nombre, recompensaCrearDto.Descripcion,
            recompensaCrearDto.Costo, recompensaCrearDto.CantidadDisponible, recompensaCrearDto.NivelRequerido);

        _repo.Agregar(recompensa);
        var recompensaCreadoDto = new RecompensaCreadoDto
        {
            Id = recompensa.Id,
            Nombre = recompensa.Nombre,
            Descripcion = recompensa.Descripcion,
            CantidadDisponible = recompensa.CantidadDisponible,
            Costo = recompensa.Costo,
            NivelRequerido = recompensa.NivelRequerido
        };

        return recompensaCreadoDto;
    }

    public IList<RecompensaCreadoDto> ObtenerTodos()
    {
        IList<RecompensaCreadoDto> dtos = [];
        foreach(Recompensa recompensa in _repo.ObtenerTodos())
        {
            var dto = new RecompensaCreadoDto
            {
                Id = recompensa.Id,
                Nombre = recompensa.Nombre,
                Descripcion = recompensa.Descripcion,
                CantidadDisponible = recompensa.CantidadDisponible,
                Costo = recompensa.Costo,
                NivelRequerido = recompensa.NivelRequerido
            };
            dtos.Add(dto);
        }

        return dtos;
    }

    public RecompensaCreadoDto ObtenerRecompensa(Guid id)
    {
        Recompensa? recompensa = _repo.ObtenerPorId(id);
        if(recompensa == null)
        {
            throw new RecompensaNoEncontradaException(id);
        }

        var dto = new RecompensaCreadoDto
        {
            Id = recompensa.Id,
            Nombre = recompensa.Nombre,
            Descripcion = recompensa.Descripcion,
            CantidadDisponible = recompensa.CantidadDisponible,
            Costo = recompensa.Costo,
            NivelRequerido = recompensa.NivelRequerido
        };
        return dto;
    }

    public RecompensaCreadoDto ActualizarRecompensa(Guid id, RecompensaCrearDto recompensaCreadoDto)
    {
        Recompensa recompensaBd = _repo.ObtenerPorId(id) ?? throw new RecompensaNoEncontradaException(id);
        recompensaBd.Nombre = recompensaCreadoDto.Nombre;
        recompensaBd.Descripcion = recompensaCreadoDto.Descripcion;
        recompensaBd.Costo = recompensaCreadoDto.Costo;
        recompensaBd.CantidadDisponible = recompensaCreadoDto.CantidadDisponible;
        recompensaBd.NivelRequerido = recompensaCreadoDto.NivelRequerido;
        _repo.Actualizar(recompensaBd);
        var dto = new RecompensaCreadoDto
        {
            Id = recompensaBd.Id,
            Nombre = recompensaBd.Nombre,
            Descripcion = recompensaBd.Descripcion,
            CantidadDisponible = recompensaBd.CantidadDisponible,
            Costo = recompensaBd.Costo,
            NivelRequerido = recompensaBd.NivelRequerido
        };
        return dto;
    }

    public void EliminarRecompensa(Guid id)
    {
        if(_repo.ObtenerPorId(id) == null)
        {
            throw new RecompensaNoEncontradaException(id);
        }

        _repo.Eliminar(id);
    }
}
