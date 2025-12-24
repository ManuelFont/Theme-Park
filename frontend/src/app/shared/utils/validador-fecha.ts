export function validarFechaNacimiento(fechaNacimiento: string): string | null {
  if (!fechaNacimiento) return 'Debes ingresar tu fecha de nacimiento.';

  const fecha = new Date(fechaNacimiento);
  const hoy = new Date();

  if (fecha > hoy) return 'La fecha de nacimiento no puede ser futura.';

  return null;
}
