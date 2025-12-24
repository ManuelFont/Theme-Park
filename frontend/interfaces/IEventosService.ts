import { Observable } from 'rxjs';
import Evento from '../models/Evento/Evento.model';
import CrearEventoRequest from '../models/Evento/CrearEventoRequest.model';

export interface IEventosService {
  getAll(): Observable<Evento[]>;
  getById(id: string): Observable<Evento>;
  create(data: CrearEventoRequest): Observable<Evento>;
  delete(id: string): Observable<void>;
  addAttraction(eventoId: string, atraccionId: string): Observable<void>;
  addMultipleAttractions(eventoId: string, atraccionesIds: string[], onDone: () => void): void;
}
