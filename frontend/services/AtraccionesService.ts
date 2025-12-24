import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import Atraccion from '../models/Atraccion/Atraccion.model';
import { environment } from '../src/environments/environment.development';
import { IAtraccionesService } from '../interfaces/IAtraccionesService.interface';
import CrearAtraccionRequest from '../models/Atraccion/CrearAtraccionRequest.model';

@Injectable({ providedIn: 'root' })
export class AtraccionesService implements IAtraccionesService {
  private readonly apiUrl = `${environment.apiUrl}/atracciones`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Atraccion[]> {
    return this.http.get<Atraccion[]>(this.apiUrl);
  }

  getById(id: string): Observable<Atraccion> {
    return this.http.get<Atraccion>(`${this.apiUrl}/${id}`);
  }

  create(data: CrearAtraccionRequest): Observable<Atraccion> {
    return this.http.post<Atraccion>(this.apiUrl, data);
  }

  update(id: string, data: CrearAtraccionRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, data);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
