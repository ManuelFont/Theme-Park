namespace Dtos;

public class CanjeCreadoDto
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid RecompensaId { get; set; }
    public string? RecompensaNombre { get; set; }
    public int? PuntosCanjeados { get; set; }
    public DateTime FechaCanje { get; set; }
}
