export interface UsuarioFormOutput {
  nombre: string;
  apellido: string;
  email: string;
  contrasenia: string | null;
  fechaNacimiento?: string | null;
  nivelMembresia?: 'Estandar' | 'Premium' | 'Vip';
  tipoUsuario?: string;
}
