import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import MantenimientoPreventivo from '../models/MantenimientoPreventivo/MantenimientoPreventivo.model';
import CrearMantenimientoPreventivoRequest from '../models/MantenimientoPreventivo/CrearMantenimientoPreventivoRequest.model';
import { environment } from '../src/environments/environment.development';
import IMantenimientosPreventivosService from '../interfaces/IMantenimientosPreventivosService.interface';

@Injectable({ providedIn: 'root' })
export class MantenimientosPreventivosService implements IMantenimientosPreventivosService {
  private readonly apiUrl = `${environment.apiUrl}/mantenimientos`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<MantenimientoPreventivo[]> {
    return this.http.get<MantenimientoPreventivo[]>(this.apiUrl);
  }

  getById(id: string): Observable<MantenimientoPreventivo> {
    return this.http.get<MantenimientoPreventivo>(`${this.apiUrl}/${id}`);
  }

  create(data: CrearMantenimientoPreventivoRequest): Observable<MantenimientoPreventivo> {
    return this.http.post<MantenimientoPreventivo>(this.apiUrl, data);
  }
}
