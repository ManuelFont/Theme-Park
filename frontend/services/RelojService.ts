import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../src/environments/environment.development';
import Reloj from '../models/Reloj.model';
import { IRelojService } from '../interfaces/IRelojService.interface';

@Injectable({ providedIn: 'root' })
export class RelojService implements IRelojService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/relojes`;

  obtenerFechaHoraActual(): Observable<Reloj> {
    return this.http.get<Reloj>(this.apiUrl);
  }
}
