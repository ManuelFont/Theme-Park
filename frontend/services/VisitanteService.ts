import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../src/environments/environment.development';
import { IVisitanteService } from '../interfaces/IVisitanteService.interface';
import HistorialPuntuacion from '../models/HistorialPuntuacion.model';
import VisitanteInfo from '../models/VisitanteInfo.model';

@Injectable({ providedIn: 'root' })
export class VisitanteService implements IVisitanteService {
  private readonly http = inject(HttpClient);

  private readonly usuarioUrl = `${environment.apiUrl}/usuarios`;
  private readonly historialUrl = `${environment.apiUrl}/historial-puntuacion`;

  getMiInformacion(): Observable<VisitanteInfo> {
    return this.http.get<VisitanteInfo>(`${this.usuarioUrl}/mi-informacion`);
  }

  getHistorialPuntuacion(): Observable<HistorialPuntuacion[]> {
    return this.http.get<HistorialPuntuacion[]>(`${this.historialUrl}/mi-historial`);
  }
}
