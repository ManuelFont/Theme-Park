import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs';
import { HotToastService } from '@ngxpert/hot-toast';
import { AtraccionesService } from '../../../../../services/AtraccionesService';
import CrearAtraccionRequest from '../../../../../models/Atraccion/CrearAtraccionRequest.model';
import AtraccionFormValue from '../../../../../models/Atraccion/AtraccionFormValue.model';
import { AtraccionForm } from '../atraccion-form/atraccion-form';
import Atraccion from '../../../../../models/Atraccion/Atraccion.model';

@Component({
  selector: 'app-crear-atraccion',
  standalone: true,
  imports: [CommonModule, AtraccionForm],
  templateUrl: './crear-atraccion.html',
  styleUrl: './crear-atraccion.css',
})
export class CrearAtraccion {
  private readonly atraccionesService = inject(AtraccionesService);
  private readonly toast = inject(HotToastService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  loading = false;
  atraccion: Atraccion | null = null;

  onSubmit(formValue: AtraccionFormValue) {
    if (this.loading) return;

    this.loading = true;

    const body: CrearAtraccionRequest = {
      nombre: formValue.nombre.trim(),
      tipo: formValue.tipo,
      edadMinima: formValue.edadMinima,
      capacidadMaxima: formValue.capacidadMaxima,
      descripcion: formValue.descripcion.trim(),
      disponible: formValue.disponible,
    };

    this.atraccionesService
      .create(body)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: () => {
          this.toast.success('Atracción creada exitosamente.');
          this.router.navigate(['/admin/atracciones']);
        },
        error: (err) => {
          console.error(err);
          this.toast.error(err.error?.message ?? 'Error al crear atracción.');
        },
      });
  }
}
