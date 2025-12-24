import { Observable } from 'rxjs';
import Reloj from '../models/Reloj.model';

export interface IRelojService {
  obtenerFechaHoraActual(): Observable<Reloj>;
}
