import { Observable } from 'rxjs';
import Recompensa from '../models/Recompensa.model';
import Canje from '../models/Canje.model';
import CanjearRecompensaRequest from '../models/CanjearRecompensaRequest.model';

export interface IRecompensasService {
  getAll(): Observable<Recompensa[]>;
  getDisponiblesParaVisitante(): Observable<Recompensa[]>;
  canjear(request: CanjearRecompensaRequest): Observable<Canje>;
  getMisCanjes(): Observable<Canje[]>;
}
