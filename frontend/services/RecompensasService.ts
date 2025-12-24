import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../src/environments/environment.development';
import { IRecompensasService } from '../interfaces/IRecompensasService.interface';
import Recompensa from '../models/Recompensa.model';
import Canje from '../models/Canje.model';
import CanjearRecompensaRequest from '../models/CanjearRecompensaRequest.model';

@Injectable({ providedIn: 'root' })
export class RecompensasService implements IRecompensasService {
  private readonly http = inject(HttpClient);

  private readonly recompensaUrl = `${environment.apiUrl}/recompensa`;
  private readonly canjeUrl = `${environment.apiUrl}/canje`;

  getAll(): Observable<Recompensa[]> {
    return this.http.get<Recompensa[]>(this.recompensaUrl);
  }

  getDisponiblesParaVisitante(): Observable<Recompensa[]> {
    return this.http.get<Recompensa[]>(this.recompensaUrl);
  }

  canjear(request: CanjearRecompensaRequest): Observable<Canje> {
    return this.http.post<Canje>(this.canjeUrl, request);
  }

  getMisCanjes(): Observable<Canje[]> {
    return this.http.get<Canje[]>(this.canjeUrl);
  }
}
