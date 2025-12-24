namespace Dominio.Exceptions;

public class RecompensaNoEncontradaException(Guid id) : Exception($"No existe recompensa con el id {id}")
{
}
