namespace ParqueTematico.BusinessLogicInterface;

public interface IComandoAccesoAtraccion
{
    Guid RegistrarIngreso(Guid ticketId, Guid atraccionId);

    void RegistrarEgreso(Guid accesoId);

    void AsignarPuntos(Guid accesoId, int puntos);
}
