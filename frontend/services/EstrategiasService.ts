import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../src/environments/environment.development';
import Estrategia from '../models/Estrategia/Estrategia.model';
import CambiarEstrategiaRequest from '../models/Estrategia/CambiarEstrategiaRequest.model';

@Injectable({
  providedIn: 'root',
})
export class EstrategiasService {
  private baseUrl = environment.apiUrl + '/estrategias';

  constructor(private http: HttpClient) {}

  obtenerTodas() {
    return this.http.get<Estrategia[]>(this.baseUrl);
  }

  obtenerActiva() {
    return this.http.get<Estrategia>(`${this.baseUrl}/activa`);
  }

  cambiar(nombre: string) {
    const body: CambiarEstrategiaRequest = {
      nombreEstrategia: nombre,
    };
    return this.http.put(`${this.baseUrl}/activa`, body);
  }
}
