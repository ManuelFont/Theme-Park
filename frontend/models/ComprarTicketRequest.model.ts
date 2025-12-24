import { TipoEntrada } from './Ticket.model';

export default interface ComprarTicketRequest {
  FechaVisita: string;
  TipoEntrada: TipoEntrada;
  EventoId?: string;
}
