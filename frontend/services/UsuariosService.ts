import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import IUsuariosService from '../interfaces/IUsuariosService.interface';
import Usuario from '../models/Usuario/Usuario.model';
import { environment } from '../src/environments/environment.development';
import ActualizarUsuarioRequest from '../models/Usuario/ActualizarUsuarioRequest.model';

@Injectable({ providedIn: 'root' })
export class UsuariosService implements IUsuariosService {
  private readonly apiUrl = `${environment.apiUrl}/usuarios`;

  constructor(private readonly http: HttpClient) {}
  getById(id: string) {
    return this.http.get<Usuario>(`${this.apiUrl}/${id}`);
  }

  update(id: string, request: ActualizarUsuarioRequest) {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request);
  }
  getAll(): Observable<Usuario[]> {
    return this.http.get<Usuario[]>(this.apiUrl);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
