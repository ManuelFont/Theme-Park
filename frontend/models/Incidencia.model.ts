import Atraccion from './Atraccion.model';

export default interface Incidencia {
  id: string;
  atraccionId: string;
  tipoIncidencia: string;
  descripcion: string;
  estaActiva: boolean;
}
