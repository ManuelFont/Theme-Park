export default interface VisitanteInfo {
  id: string;
  nombre: string;
  apellido: string;
  email: string;
  nivelMembresia: 'Estandar' | 'Premium' | 'VIP';
  puntosActuales: number;
}
