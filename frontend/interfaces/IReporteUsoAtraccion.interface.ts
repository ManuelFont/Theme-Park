import ReporteUsoAtraccion from '../models/ReporteUsoAtraccion.model';
import { Observable } from 'rxjs';

export default interface IReporteUsoAtraccionService {
  obtenerReporte(fechaInicio: string, fechaFin: string): Observable<ReporteUsoAtraccion[]>;
}
