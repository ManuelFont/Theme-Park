export default interface Usuario {
  id: string;
  nombre: string;
  apellido: string;
  email: string;
  tipoUsuario: 'Administrador' | 'Operador' | 'Visitante';
  nivelMembresia?: 'Estandar' | 'Premium' | 'VIP' | null;
  fechaNacimiento?: string | null;
}
