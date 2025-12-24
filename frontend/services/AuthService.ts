import { Observable } from 'rxjs';
import { IAuthService } from '../interfaces/IAuthService.interface';
import LoginResponse from '../models/Auth/LoginResponse.model';
import { environment } from '../src/environments/environment.development';
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import CrearUsuarioRequest from '../models/Usuario/CrearUsuarioRequest.model';
import RegisterResponse from '../models/Auth/RegisterResponse.model';
import LoginRequest from '../models/Auth/LoginRequest.model';

@Injectable({ providedIn: 'root' })
export class AuthService implements IAuthService {
  private readonly apiUrl: string = environment.apiUrl;
  private readonly http = inject(HttpClient);

  login(data: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(this.apiUrl + '/auth/login', data);
  }

  register(data: CrearUsuarioRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(this.apiUrl + '/usuarios', data, {
      headers: { 'Content-Type': 'application/json; charset=utf-8' },
    });
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('expiraEn');
    localStorage.removeItem('tipoUsuario');
  }

  isAuthenticated(): boolean {
    const token = localStorage.getItem('token');
    const expiraEn = localStorage.getItem('expiraEn');
    if (!token || !expiraEn) {
      return false;
    }
    return new Date(expiraEn) > new Date();
  }

  saveSession(token: string, expiraEn: string): void {
    const userInfo = JSON.parse(atob(token.split('.')[1]));
    localStorage.setItem('token', token);
    localStorage.setItem('expiraEn', expiraEn);
    localStorage.setItem('tipoUsuario', userInfo.tipoUsuario);
  }

  getUserRole(): string {
    return localStorage.getItem('tipoUsuario') ?? '';
  }

  getCurrentUser(): string | null {
    const token = localStorage.getItem('token');
    if (!token) return null;
    const userInfo = JSON.parse(atob(token.split('.')[1]));
    const userEmail = userInfo.email;
    return userEmail;
  }

  getUserId(): string | null {
    const token = localStorage.getItem('token');
    if (!token) return null;
    const userInfo = JSON.parse(atob(token.split('.')[1]));
    return userInfo.id;
  }
}
