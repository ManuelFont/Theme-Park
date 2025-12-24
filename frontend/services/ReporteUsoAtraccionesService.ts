import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import IReporteUsoAtraccionService from '../interfaces/IReporteUsoAtraccion.interface';
import ReporteUsoAtraccion from '../models/ReporteUsoAtraccion.model';
import { environment } from '../src/environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class ReporteUsoAtraccionesService implements IReporteUsoAtraccionService {
  private readonly apiUrl = `${environment.apiUrl}/accesos/reporte-uso`;

  constructor(private http: HttpClient) {}

  obtenerReporte(fechaInicio: string, fechaFin: string): Observable<ReporteUsoAtraccion[]> {
    return this.http.get<ReporteUsoAtraccion[]>(
      `${this.apiUrl}?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`
    );
  }
}
