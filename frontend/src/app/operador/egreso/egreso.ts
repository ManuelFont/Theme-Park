import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HotToastService } from '@ngxpert/hot-toast';
import { ZardButtonComponent } from '@shared/components/button/button.component';
import { ZardCardComponent } from '@shared/components/card/card.component';
import { AccesoAtraccionService } from '../../../../services/AccesoAtraccionService';
import AccesoAtraccion from '../../../../models/AccesoAtraccion.model';

@Component({
  selector: 'app-egreso',
  standalone: true,
  templateUrl: './egreso.html',
  imports: [CommonModule, FormsModule, ZardButtonComponent, ZardCardComponent],
})
export class Egreso {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private toast = inject(HotToastService);
  private accesoService = inject(AccesoAtraccionService);

  atraccionId = '';
  accesoId = '';

  cargando = false;
  acceso: AccesoAtraccion | null = null;
  error: string | null = null;
  resultado: string | null = null;

  ngOnInit() {
    this.atraccionId = this.route.snapshot.paramMap.get('id')!;
  }

  buscarAcceso() {
    this.error = null;
    this.resultado = null;
    this.acceso = null;

    if (!this.accesoId.trim()) {
      this.error = 'Debe ingresar un ID de acceso.';
      this.toast.error(this.error);
      return;
    }

    this.cargando = true;

    this.accesoService.obtenerPorId(this.accesoId).subscribe({
      next: (acc) => {
        this.acceso = acc;
        this.cargando = false;
      },
      error: () => {
        this.cargando = false;
        this.error = 'No se encontró el acceso.';
        this.toast.error(this.error);
      },
    });
  }

  registrarEgreso() {
    if (!this.acceso) return;

    if (this.acceso.fechaHoraEgreso) {
      this.toast.error('El egreso ya fue registrado.');
      return;
    }

    this.cargando = true;

    this.accesoService.registrarEgreso(this.acceso.id).subscribe({
      next: () => {
        this.cargando = false;
        this.resultado = 'Egreso registrado correctamente.';
        this.toast.success(this.resultado);

        this.buscarAcceso();
      },
      error: (err) => {
        this.cargando = false;
        const mensaje = err.error?.message || 'Error al registrar el egreso.';
        this.error = mensaje;
        this.toast.error(mensaje);
      },
    });
  }

  volver() {
    this.router.navigate(['/operador/atraccion', this.atraccionId]);
  }
}
