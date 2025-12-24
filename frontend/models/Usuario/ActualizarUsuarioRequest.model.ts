export default interface ActualizarUsuarioRequest {
  nombre: string;
  apellido: string;
  email: string;
  contrasenia: string | null;
  fechaNacimiento?: string | null;
  nivelMembresia?: 'Estandar' | 'Premium' | 'Vip' | null;
}
