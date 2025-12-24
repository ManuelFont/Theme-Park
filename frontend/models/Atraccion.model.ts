export default interface Atraccion {
  id: string;
  nombre: string;
  descripcion: string;
  tipo: string;
  edadMinima: number;
  capacidadMaxima: number;
  disponible: boolean;
}
