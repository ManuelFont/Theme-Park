import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HotToastService } from '@ngxpert/hot-toast';
import { ZardIconComponent } from '@shared/components/icon/icon.component';
import { Gift, Award, Package, Lock } from 'lucide-angular';
import { RecompensasService } from '../../../../services/RecompensasService';
import { VisitanteService } from '../../../../services/VisitanteService';
import Recompensa from '../../../../models/Recompensa.model';
import VisitanteInfo from '../../../../models/VisitanteInfo.model';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-recompensas',
  standalone: true,
  imports: [CommonModule, ZardIconComponent],
  templateUrl: './recompensas.html',
  styleUrl: './recompensas.css',
})
export class Recompensas implements OnInit {
  GiftIcon = Gift;
  AwardIcon = Award;
  PackageIcon = Package;
  LockIcon = Lock;

  recompensas: Recompensa[] = [];
  visitante: VisitanteInfo | null = null;
  loading = false;
  canjeando: string | null = null;

  private readonly recompensasService = inject(RecompensasService);
  private readonly visitanteService = inject(VisitanteService);
  private readonly toast = inject(HotToastService);

  ngOnInit() {
    this.cargarRecompensas();
    this.cargarPerfil();
  }

  cargarRecompensas() {
    this.loading = true;
    this.recompensasService
      .getAll()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (data) => (this.recompensas = data),
        error: () => this.toast.error('Error al cargar recompensas'),
      });
  }

  cargarPerfil() {
    this.visitanteService.getMiInformacion().subscribe({
      next: (data) => (this.visitante = data),
      error: () => this.toast.error('Error al cargar perfil'),
    });
  }

  puedeCanjear(recompensa: Recompensa): boolean {
    if (!this.visitante) return false;
    if (recompensa.cantidadDisponible <= 0) return false;
    if (this.visitante.puntosActuales < recompensa.costo) return false;
    if (!this.tieneNivelSuficiente(recompensa)) return false;
    return true;
  }

  tieneNivelSuficiente(recompensa: Recompensa): boolean {
    if (!recompensa.nivelMembresia || !this.visitante) return true;

    const niveles = { 'Estandar': 1, 'Premium': 2, 'VIP': 3 };
    const nivelRequerido = niveles[recompensa.nivelMembresia] || 0;
    const nivelVisitante = niveles[this.visitante.nivelMembresia] || 0;

    return nivelVisitante >= nivelRequerido;
  }

  motivoNoPuedeCanjear(recompensa: Recompensa): string {
    if (!this.visitante) return '';
    if (recompensa.cantidadDisponible <= 0) return 'Sin stock';
    if (this.visitante.puntosActuales < recompensa.costo) {
      return `Te faltan ${recompensa.costo - this.visitante.puntosActuales} puntos`;
    }
    if (!this.tieneNivelSuficiente(recompensa)) {
      return `Requiere membresía ${recompensa.nivelMembresia}`;
    }
    return '';
  }

  canjear(recompensa: Recompensa) {
    if (!this.puedeCanjear(recompensa) || this.canjeando) return;

    this.canjeando = recompensa.id;
    this.recompensasService
      .canjear({ recompensaId: recompensa.id })
      .pipe(finalize(() => (this.canjeando = null)))
      .subscribe({
        next: () => {
          this.toast.success('Recompensa canjeada exitosamente');
          recompensa.cantidadDisponible--;
          if (this.visitante) {
            this.visitante.puntosActuales -= recompensa.costo;
          }
        },
        error: (err) => {
          const mensaje = err.error?.message || 'Error al canjear recompensa';
          this.toast.error(mensaje);
        },
      });
  }
}
