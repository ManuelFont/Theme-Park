import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../src/environments/environment.development';
import Incidencia from '../models/Incidencia.model';

@Injectable({
  providedIn: 'root',
})
export class IncidenciasService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/incidencias`;

  crear(body: {
    atraccionId: string;
    tipoIncidencia: string;
    descripcion: string;
  }): Observable<Incidencia> {
    return this.http.post<Incidencia>(this.baseUrl, body);
  }

  cerrar(id: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/cerrar`, {});
  }

  obtenerPorAtraccion(atraccionId: string): Observable<Incidencia[]> {
    return this.http.get<Incidencia[]>(`${this.baseUrl}`, {
      params: {
        atraccionId,
        activas: true,
      },
    });
  }

  existeActiva(atraccionId: string): Observable<boolean> {
    return this.http.get<boolean>(`${this.baseUrl}/existe-activa`, {
      params: { atraccionId },
    });
  }
}
