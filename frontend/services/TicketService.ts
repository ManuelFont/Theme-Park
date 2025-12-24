import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../src/environments/environment.development';
import Ticket from '../models/Ticket.model';
import ComprarTicketRequest from '../models/ComprarTicketRequest.model';
import { ITicketService } from '../interfaces/ITicketService.interface';

@Injectable({ providedIn: 'root' })
export class TicketService implements ITicketService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/tickets`;

  comprarTicket(request: ComprarTicketRequest): Observable<{ ticketId: string }> {
    return this.http.post<{ ticketId: string }>(this.apiUrl, request);
  }

  obtenerPorId(id: string): Observable<Ticket> {
    return this.http.get<Ticket>(`${this.apiUrl}/${id}`);
  }
}
