import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HotToastService } from '@ngxpert/hot-toast';
import { ZardIconComponent } from '@shared/components/icon/icon.component';
import { Gift, Calendar, Award } from 'lucide-angular';
import { RecompensasService } from '../../../../services/RecompensasService';
import Canje from '../../../../models/Canje.model';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-mis-canjes',
  standalone: true,
  imports: [CommonModule, ZardIconComponent],
  templateUrl: './mis-canjes.html',
  styleUrl: './mis-canjes.css',
})
export class MisCanjes implements OnInit {
  GiftIcon = Gift;
  CalendarIcon = Calendar;
  AwardIcon = Award;

  canjes: Canje[] = [];
  loading = false;

  private readonly recompensasService = inject(RecompensasService);
  private readonly toast = inject(HotToastService);

  ngOnInit() {
    this.cargarCanjes();
  }

  cargarCanjes() {
    this.loading = true;
    this.recompensasService
      .getMisCanjes()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (data) => (this.canjes = data),
        error: () => this.toast.error('Error al cargar historial de canjes'),
      });
  }
}
