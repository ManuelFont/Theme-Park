export default interface CrearUsuarioRequest {
  nombre: string;
  apellido: string;
  email: string;
  contrasenia: string;
  tipoUsuario: string;
  fechaNacimiento?: string | null;
  nivelMembresia?: 'Estandar' | 'Premium' | 'Vip' | null;
}
