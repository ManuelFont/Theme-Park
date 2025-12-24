import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../src/environments/environment.development';
import Evento from '../models/Evento.model';
import { IEventoService } from '../interfaces/IEventoService.interface';

@Injectable({ providedIn: 'root' })
export class EventoService implements IEventoService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/eventos`;

  getAll(): Observable<Evento[]> {
    return this.http.get<Evento[]>(this.apiUrl);
  }
}
