import { Observable } from 'rxjs';
import Atraccion from '../models/Atraccion/Atraccion.model';
import CrearAtraccionRequest from '../models/Atraccion/CrearAtraccionRequest.model';

export interface IAtraccionesService {
  getAll(): Observable<Atraccion[]>;
  getById(id: string): Observable<Atraccion>;
  create(data: CrearAtraccionRequest): Observable<Atraccion>;
  update(id: string, data: CrearAtraccionRequest): Observable<void>;
  delete(id: string): Observable<void>;
}
