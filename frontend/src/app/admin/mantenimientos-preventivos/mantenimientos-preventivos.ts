import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { HotToastService } from '@ngxpert/hot-toast';
import { ZardButtonComponent } from '@shared/components/button/button.component';
import { ZardIconComponent } from '@shared/components/icon/icon.component';
import { CircleCheck } from 'lucide-angular';
import MantenimientoPreventivo from '../../../../models/MantenimientoPreventivo/MantenimientoPreventivo.model';
import { MantenimientosPreventivosService } from '../../../../services/MantenimientosPreventivosService';

@Component({
  selector: 'app-mantenimientos-preventivos',
  imports: [CommonModule, ZardButtonComponent],
  templateUrl: './mantenimientos-preventivos.html',
  styleUrl: './mantenimientos-preventivos.css',
})
export class MantenimientoPreventivos implements OnInit {
  CircleCheck = CircleCheck;

  mantenimientos: MantenimientoPreventivo[] = [];

  private readonly service = inject(MantenimientosPreventivosService);
  private readonly toast = inject(HotToastService);
  private readonly router = inject(Router);

  ngOnInit(): void {
    this.cargar();
  }

  cargar() {
    this.service.getAll().subscribe({
      next: (data) => {
        this.mantenimientos = data;
      },
      error: () => this.toast.error('Error al cargar mantenimientos'),
    });
  }

  crearMantenimiento() {
    this.router.navigate(['/admin/mantenimientos-preventivos/crear']);
  }

  getEstado(m: MantenimientoPreventivo): string {
    const now = new Date();
    const inicio = new Date(m.fechaInicio);
    const fin = new Date(m.fechaFin);

    if (now < inicio) return 'Programado';
    if (now >= inicio && now <= fin) return 'En curso';
    return 'Finalizado';
  }
}
