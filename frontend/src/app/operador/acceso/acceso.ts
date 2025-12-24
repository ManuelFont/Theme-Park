import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ZardButtonComponent } from '@shared/components/button/button.component';
import { HotToastService } from '@ngxpert/hot-toast';
import { ZardCardComponent } from '@shared/components/card/card.component';
import { AccesoAtraccionService } from '../../../../services/AccesoAtraccionService';

@Component({
  selector: 'app-acceso',
  standalone: true,
  templateUrl: './acceso.html',
  imports: [CommonModule, FormsModule, ZardButtonComponent, ZardCardComponent],
})
export class AccesoComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private toast = inject(HotToastService);
  private accesoService = inject(AccesoAtraccionService);

  atraccionId: string = '';
  ticketId: string = '';

  cargando = false;
  resultado: string | null = null;
  error: string | null = null;

  ngOnInit() {
    this.atraccionId = this.route.snapshot.paramMap.get('id')!;
  }

  registrarIngreso() {
    this.cargando = true;
    this.resultado = null;
    this.error = null;

    this.accesoService.registrarIngreso(this.ticketId, this.atraccionId).subscribe({
      next: (res) => {
        this.cargando = false;
        this.resultado = `Ingreso registrado con éxito. ID de acceso: ${res.accesoId}`;
        this.ticketId = '';
      },
      error: (err) => {
        this.cargando = false;
        this.toast.error(err.error?.message || 'Error al registrar el ingreso.');
      },
    });
  }

  volver() {
    this.router.navigate(['/operador/atraccion', this.atraccionId]);
  }
}
