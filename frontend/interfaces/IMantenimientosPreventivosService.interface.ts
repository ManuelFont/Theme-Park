import { Observable } from 'rxjs';
import MantenimientoPreventivo from '../models/MantenimientoPreventivo/MantenimientoPreventivo.model';
import CrearMantenimientoPreventivoRequest from '../models/MantenimientoPreventivo/CrearMantenimientoPreventivoRequest.model';

export default interface IMantenimientosPreventivosService {
  getAll(): Observable<MantenimientoPreventivo[]>;
  getById(id: string): Observable<MantenimientoPreventivo>;
  create(data: CrearMantenimientoPreventivoRequest): Observable<MantenimientoPreventivo>;
}
