import { Observable } from 'rxjs';
import AccesoAtraccion from '../models/AccesoAtraccion.model';

export interface IAccesoAtraccionService {
  obtenerMiHistorial(fecha: string): Observable<AccesoAtraccion[]>;
}
