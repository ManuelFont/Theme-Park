import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HotToastService } from '@ngxpert/hot-toast';
import { ZardIconComponent } from '@shared/components/icon/icon.component';
import { Calendar, Award, TrendingUp } from 'lucide-angular';
import { HistorialPuntuacionService } from '../../../../services/HistorialPuntuacionService';
import HistorialPuntuacion from '../../../../models/HistorialPuntuacion.model';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-historial-puntuacion',
  standalone: true,
  imports: [CommonModule, ZardIconComponent],
  templateUrl: './historial-puntuacion.html',
  styleUrl: './historial-puntuacion.css',
})
export class HistorialPuntuacionComponent implements OnInit {
  CalendarIcon = Calendar;
  AwardIcon = Award;
  TrendingUpIcon = TrendingUp;

  historial: HistorialPuntuacion[] = [];
  loading = false;

  private readonly historialService = inject(HistorialPuntuacionService);
  private readonly toast = inject(HotToastService);

  ngOnInit() {
    this.cargarHistorial();
  }

  cargarHistorial() {
    this.loading = true;
    this.historialService
      .getMiHistorial()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (data) => (this.historial = data),
        error: () => this.toast.error('Error al cargar historial de puntuación'),
      });
  }
}
