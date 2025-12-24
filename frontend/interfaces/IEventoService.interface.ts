import { Observable } from 'rxjs';
import Evento from '../models/Evento.model';

export interface IEventoService {
  getAll(): Observable<Evento[]>;
}
