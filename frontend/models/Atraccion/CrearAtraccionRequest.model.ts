export default interface CrearAtraccionRequest {
  nombre: string;
  tipo: 'MontañaRusa' | 'Simulador' | 'Espectaculo' | 'ZonaInteractiva';
  edadMinima: number;
  capacidadMaxima: number;
  descripcion: string;
  disponible: boolean;
}
