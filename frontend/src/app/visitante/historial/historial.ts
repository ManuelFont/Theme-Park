import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HotToastService } from '@ngxpert/hot-toast';
import { ZardIconComponent } from '@shared/components/icon/icon.component';
import { Calendar, Clock, Award } from 'lucide-angular';
import { AccesoAtraccionService } from '../../../../services/AccesoAtraccionService';
import { RelojService } from '../../../../services/RelojService';
import AccesoAtraccion from '../../../../models/AccesoAtraccion.model';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-historial',
  standalone: true,
  imports: [CommonModule, FormsModule, ZardIconComponent],
  templateUrl: './historial.html',
  styleUrl: './historial.css',
})
export class Historial implements OnInit {
  CalendarIcon = Calendar;
  ClockIcon = Clock;
  AwardIcon = Award;

  accesos: AccesoAtraccion[] = [];
  loading = false;
  fechaSeleccionada: string = '';
  fechaMaxima: string = '';

  private readonly accesoService = inject(AccesoAtraccionService);
  private readonly relojService = inject(RelojService);
  private readonly toast = inject(HotToastService);

  ngOnInit() {
    this.cargarFechaActual();
  }

  cargarFechaActual() {
    this.relojService.obtenerFechaHoraActual().subscribe({
      next: (reloj) => {
        this.fechaMaxima = reloj.fechaHora.split('T')[0];
        this.fechaSeleccionada = this.fechaMaxima;
        this.cargarHistorial();
      },
      error: () => {
        const hoy = new Date();
        this.fechaMaxima = hoy.toISOString().split('T')[0];
        this.fechaSeleccionada = this.fechaMaxima;
        this.cargarHistorial();
      },
    });
  }

  cargarHistorial() {
    if (!this.fechaSeleccionada) return;

    this.loading = true;
    this.accesoService
      .obtenerMiHistorial(this.fechaSeleccionada)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (data) => (this.accesos = data),
        error: () => this.toast.error('Error al cargar historial'),
      });
  }

  calcularDuracion(fechaIngreso: string, fechaEgreso: string | null): string {
    if (!fechaEgreso) return 'En curso';

    const ingreso = new Date(fechaIngreso);
    const egreso = new Date(fechaEgreso);
    const diffMs = egreso.getTime() - ingreso.getTime();
    const diffMinutes = Math.floor(diffMs / 60000);

    if (diffMinutes < 60) {
      return `${diffMinutes} min`;
    }

    const hours = Math.floor(diffMinutes / 60);
    const minutes = diffMinutes % 60;
    return `${hours}h ${minutes}min`;
  }
}
