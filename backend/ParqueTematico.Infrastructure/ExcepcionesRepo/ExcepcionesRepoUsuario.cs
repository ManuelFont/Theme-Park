namespace Infrastructure.ExcepcionesRepo;

public class ExcepcionRepositorioUsuario(string mensaje) : Exception(mensaje)
{
}
