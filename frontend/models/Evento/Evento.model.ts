import Atraccion from '../Atraccion/Atraccion.model';

export default interface Evento {
  id: string;
  nombre: string;
  fecha: string;
  hora: string;
  aforo: number;
  costoAdicional: number;
  atracciones: Atraccion[];
}
