namespace Infrastructure.ExcepcionesRepo;

public class ExcepcionRepositorioTicket(string mensaje) : Exception(mensaje)
{
}
