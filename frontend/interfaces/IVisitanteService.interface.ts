import { Observable } from 'rxjs';
import HistorialPuntuacion from '../models/HistorialPuntuacion.model';
import VisitanteInfo from '../models/VisitanteInfo.model';

export interface IVisitanteService {
  getMiInformacion(): Observable<VisitanteInfo>;
  getHistorialPuntuacion(): Observable<HistorialPuntuacion[]>;
}
