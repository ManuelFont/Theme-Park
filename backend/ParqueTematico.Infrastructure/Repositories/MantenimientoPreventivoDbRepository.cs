using Dominio.Entities;
using RepositoryInterfaces;

namespace Infrastructure.Repositories;

public class MantenimientoPreventivoDbRepository(ParqueDbContext context) : BaseRepository<MantenimientoPreventivo>(context), IMantenimientoPreventivoRepository
{
}
