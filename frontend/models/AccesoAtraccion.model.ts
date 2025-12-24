export default interface AccesoAtraccion {
  id: string;
  visitante: {
    id: string;
    nombre: string;
    apellido: string;
    email: string;
  };
  atraccion: {
    id: string;
    nombre: string;
    descripcion: string;
    aforoMaximo: number;
    edadMinima: number;
    alturaMinima: number;
    enMantenimiento: boolean;
  };
  ticketId: string;
  fechaHoraIngreso: string;
  fechaHoraEgreso: string | null;
  puntosObtenidos: number;
}
