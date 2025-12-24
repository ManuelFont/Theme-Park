import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../src/environments/environment.development';
import HistorialPuntuacion from '../models/HistorialPuntuacion.model';

@Injectable({ providedIn: 'root' })
export class HistorialPuntuacionService {
  private readonly http = inject(HttpClient);

  private readonly historialUrl = `${environment.apiUrl}/historial-puntuacion`;

  getMiHistorial(): Observable<HistorialPuntuacion[]> {
    return this.http.get<HistorialPuntuacion[]>(`${this.historialUrl}/mi-historial`);
  }
}
