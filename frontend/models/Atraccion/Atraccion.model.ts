import Incidencia from '../Incidencia.model';

export default interface Atraccion {
  id: string;
  nombre: string;
  tipo: 'MontañaRusa' | 'Simulador' | 'Espectáculo' | 'ZonaInteractiva';
  edadMinima: number;
  capacidadMaxima: number;
  descripcion: string;
  disponible: boolean;
  incidencias?: Incidencia[];
  aforoActual?: number;
}
