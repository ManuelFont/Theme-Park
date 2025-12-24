import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../src/environments/environment.development';
import Incidencia from '../models/Incidencia.model';
import RegistrarIngresoResponse from '../models/RegistrarIngresoResponse.model';
import RegistrarIngresoTicketRequest from '../models/RegistrarIngresoTicketRequest.model';
import RegistrarIngresoNfcRequest from '../models/RegistrarIngresoNfcRequest.model';
import CrearIncidenciaRequest from '../models/CrearIncidenciaRequest.model';
@Injectable({
  providedIn: 'root',
})
export class OperadorService {
  private api = environment.apiUrl + '/api';

  private http = inject(HttpClient);

  registrarIngresoPorTicket(
    ticketId: string,
    atraccionId: string
  ): Observable<RegistrarIngresoResponse> {
    const body: RegistrarIngresoTicketRequest = { ticketId, atraccionId };
    return this.http.post<RegistrarIngresoResponse>(`${this.api}/accesos/ingreso`, body);
  }

  registrarIngresoPorNfc(
    visitanteNfcId: string,
    atraccionId: string
  ): Observable<RegistrarIngresoResponse> {
    const body = { visitanteNfcId, atraccionId };
    return this.http.post<RegistrarIngresoResponse>(`${this.api}/accesos/ingreso-nfc`, body);
  }

  registrarEgreso(accesoId: string): Observable<{ mensaje: string }> {
    return this.http.put<{ mensaje: string }>(`${this.api}/accesos/egreso/${accesoId}`, {});
  }

  obtenerIncidenciasActivas(atraccionId: string): Observable<Incidencia[]> {
    return this.http.get<Incidencia[]>(
      `${this.api}/incidencias?atraccionId=${atraccionId}&activas=true`
    );
  }

  crearIncidencia(
    atraccionId: string,
    tipoIncidencia: string,
    descripcion: string
  ): Observable<Incidencia> {
    const body: CrearIncidenciaRequest = { atraccionId, tipoIncidencia, descripcion };
    return this.http.post<Incidencia>(`${this.api}/incidencias`, body);
  }

  cerrarIncidencia(id: string): Observable<void> {
    return this.http.put<void>(`${this.api}/incidencias/${id}/cerrar`, {});
  }
}
