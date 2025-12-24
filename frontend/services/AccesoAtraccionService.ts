import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

import { environment } from '../src/environments/environment.development';
import AccesoAtraccion from '../models/AccesoAtraccion.model';
import { IAccesoAtraccionService } from '../interfaces/IAccesoAtraccionService.interface';

@Injectable({ providedIn: 'root' })
export class AccesoAtraccionService implements IAccesoAtraccionService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/accesos`;

  obtenerMiHistorial(fecha: string): Observable<AccesoAtraccion[]> {
    return this.http.get<AccesoAtraccion[]>(`${this.apiUrl}/mi-historial?fecha=${fecha}`);
  }

  obtenerPorId(id: string): Observable<AccesoAtraccion> {
    return this.http.get<AccesoAtraccion>(`${this.apiUrl}/${id}`);
  }

  registrarEgreso(accesoId: string): Observable<void> {
    return this.http
      .put<{ mensaje: string }>(`${this.apiUrl}/egreso/${accesoId}`, {})
      .pipe(map(() => void 0));
  }

  registrarIngreso(ticketId: string, atraccionId: string): Observable<{ accesoId: string }> {
    return this.http.post<{ accesoId: string }>(`${this.apiUrl}/ingreso`, {
      ticketId,
      atraccionId,
    });
  }

  obtenerAforo(atraccionId: string): Observable<number> {
    return this.http
      .get<{ aforoActual: number }>(`${this.apiUrl}/aforo/${atraccionId}`)
      .pipe(map((x) => x.aforoActual));
  }
}
