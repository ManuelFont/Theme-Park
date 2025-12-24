export default interface MantenimientoPreventivo {
  id: string;
  nombreAtraccion: string;
  descripcion: string;
  estaActiva: boolean;
  tipoIncidencia: string;
  atraccion: {
    id: string;
    nombre: string;
  };
  fechaInicio: string;
  fechaFin: string;
}
