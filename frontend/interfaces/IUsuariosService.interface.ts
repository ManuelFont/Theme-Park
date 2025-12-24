import { Observable } from 'rxjs';
import Usuario from '../models/Usuario/Usuario.model';
import ActualizarUsuarioRequest from '../models/Usuario/ActualizarUsuarioRequest.model';
export default interface IUsuariosService {
  getAll(): Observable<Usuario[]>;
  getById(id: string): Observable<Usuario>;
  delete(id: string): Observable<void>;
  update(id: string, request: ActualizarUsuarioRequest): Observable<void>;
}
