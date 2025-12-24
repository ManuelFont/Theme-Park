import { Component, inject } from '@angular/core';
import ReporteUsoAtraccion from '../../../../models/ReporteUsoAtraccion.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ZardTableComponent } from '@shared/components/table/table.component';
import { ZardButtonComponent } from '@shared/components/button/button.component';
import { ReporteUsoAtraccionesService } from '../../../../services/ReporteUsoAtraccionesService';
import { HotToastService } from '@ngxpert/hot-toast';

@Component({
  selector: 'app-reporte-uso-atracciones',
  standalone: true,
  imports: [CommonModule, FormsModule, ZardTableComponent, ZardButtonComponent],
  templateUrl: './reporte-uso-atracciones.html',
  styleUrl: './reporte-uso-atracciones.css',
})
export class ReporteUsoAtracciones {
  fechaInicio = '';
  fechaFin = '';

  cargando = false;
  reporte: ReporteUsoAtraccion[] = [];

  private reporteService = inject(ReporteUsoAtraccionesService);
  private toast = inject(HotToastService);

  buscar() {
    if (!this.fechaInicio || !this.fechaFin) {
      this.toast.error('Debe seleccionar ambas fechas.');
      return;
    }

    this.cargando = true;
    this.reporte = [];

    this.reporteService
      .obtenerReporte(this.fechaInicio, this.fechaFin)
      .pipe(
        this.toast.observe({
          loading: 'Cargando reporte...',
          success: 'Reporte cargado con éxito.',
          error: 'Ocurrió un error al obtener el reporte.',
        })
      )
      .subscribe({
        next: (data) => {
          this.reporte = data;
          this.cargando = false;
        },
        error: (err) => {
          console.log(err);
          this.cargando = false;
        },
      });
  }
}
