import { Observable } from 'rxjs';
import LoginResponse from '../models/Auth/LoginResponse.model';
import RegisterResponse from '../models/Auth/RegisterResponse.model';
import CrearUsuarioRequest from '../models/Usuario/CrearUsuarioRequest.model';
import LoginRequest from '../models/Auth/LoginRequest.model';

export interface IAuthService {
  login(data: LoginRequest): Observable<LoginResponse>;
  register(data: CrearUsuarioRequest): Observable<RegisterResponse>;
  logout(): void;
  isAuthenticated(): boolean;
  saveSession(token: string, expiraEn: string): void;
  getUserRole(): string;
  getCurrentUser(): string | null;
  getUserId(): string | null;
}
