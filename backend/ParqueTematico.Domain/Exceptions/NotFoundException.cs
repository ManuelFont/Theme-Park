namespace Dominio.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
}
