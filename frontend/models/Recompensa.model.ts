export default interface Recompensa {
  id: string;
  nombre: string;
  descripcion: string;
  costo: number;
  cantidadDisponible: number;
  nivelMembresia?: 'Estandar' | 'Premium' | 'VIP' | null;
}
