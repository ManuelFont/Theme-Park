import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import Evento from '../models/Evento/Evento.model';
import CrearEventoRequest from '../models/Evento/CrearEventoRequest.model';
import { Observable, forkJoin, map } from 'rxjs';
import { environment } from '../src/environments/environment.development';
import { IEventosService } from '../interfaces/IEventosService';

@Injectable({ providedIn: 'root' })
export class EventosService implements IEventosService {
  private readonly apiUrl = `${environment.apiUrl}/eventos`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Evento[]> {
    return this.http.get<Evento[]>(this.apiUrl);
  }

  getById(id: string): Observable<Evento> {
    return this.http.get<Evento>(`${this.apiUrl}/${id}`);
  }

  create(data: CrearEventoRequest): Observable<Evento> {
    return this.http.post<Evento>(this.apiUrl, data);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  addAttraction(eventoId: string, atraccionId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${eventoId}/atracciones/${atraccionId}`, {});
  }

  addMultipleAttractions(eventoId: string, atraccionesIds: string[]): Observable<void> {
    const requests = atraccionesIds.map((id) =>
      this.http.post<void>(`${this.apiUrl}/${eventoId}/atracciones/${id}`, {})
    );
    return forkJoin(requests).pipe(map(() => void 0));
  }
}
