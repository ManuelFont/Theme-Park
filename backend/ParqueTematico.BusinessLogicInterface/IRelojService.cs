using Dominio.Entities;

namespace ParqueTematico.BusinessLogicInterface;

public interface IRelojService
{
    Reloj ModificarFechaHora(DateTime fechaHora);

    DateTime ObtenerFechaHora();

    void Attach(IRelojObserver observer);

    void Notify();
}
