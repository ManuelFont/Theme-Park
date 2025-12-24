import { Observable } from 'rxjs';
import Ticket from '../models/Ticket.model';
import ComprarTicketRequest from '../models/ComprarTicketRequest.model';

export interface ITicketService {
  comprarTicket(request: ComprarTicketRequest): Observable<{ ticketId: string }>;
  obtenerPorId(id: string): Observable<Ticket>;
}
