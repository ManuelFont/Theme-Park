export type TipoEntrada = 'General' | 'EventoEspecial';

export default interface Ticket {
  id: string;
  visitante: {
    id: string;
    nombre: string;
    apellido: string;
    email: string;
  };
  fechaVisita: string;
  tipoEntrada: TipoEntrada;
  eventoAsociado: {
    id: string;
    nombre: string;
    fecha: string;
  } | null;
}
