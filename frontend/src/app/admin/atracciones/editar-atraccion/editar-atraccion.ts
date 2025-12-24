import { Component, inject, Input } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HotToastService } from '@ngxpert/hot-toast';
import { finalize } from 'rxjs';
import CrearAtraccionRequest from '../../../../../models/Atraccion/CrearAtraccionRequest.model';
import { AtraccionesService } from '../../../../../services/AtraccionesService';
import Atraccion from '../../../../../models/Atraccion/Atraccion.model';
import AtraccionFormValue from '../../../../../models/Atraccion/AtraccionFormValue.model';
import { AtraccionForm } from '../atraccion-form/atraccion-form';

@Component({
  selector: 'app-editar-atraccion',
  standalone: true,
  imports: [AtraccionForm],
  templateUrl: './editar-atraccion.html',
  styleUrl: './editar-atraccion.css',
})
export class EditarAtraccion {
  private readonly route = inject(ActivatedRoute);
  private readonly atraccionService = inject(AtraccionesService);
  private readonly toast = inject(HotToastService);
  private readonly router = inject(Router);

  atraccion: Atraccion | null = null;
  id!: string;
  loading = false;

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id')!;
    this.cargarAtraccion();
  }

  private cargarAtraccion() {
    this.loading = true;
    this.atraccionService
      .getById(this.id)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (data) => {
          console.log(' recibido:', data);
          this.atraccion = data;
        },
        error: () => this.toast.error('Error al cargar la atraccion'),
      });
  }

  onSubmit(formValue: AtraccionFormValue) {
    if (this.loading) return;
    this.loading = true;

    const body: CrearAtraccionRequest = {
      nombre: formValue.nombre.trim(),
      tipo: formValue.tipo,
      edadMinima: formValue.edadMinima,
      capacidadMaxima: formValue.capacidadMaxima,
      descripcion: formValue.descripcion,
      disponible: formValue.disponible,
    };

    this.atraccionService
      .update(this.id, body as CrearAtraccionRequest)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: () => {
          this.toast.success('Atraccion actualizada correctamente');
          this.router.navigate(['/admin/atracciones']);
        },
        error: (err) => {
          if (err.status === 409) {
            this.toast.error('El nombre ya está en uso.');
          } else if (err.status === 400) {
            this.toast.error('Datos inválidos. Revisá los campos.');
          } else if (err.status === 404) {
            this.toast.error('Atracción no encontrada.');
          } else {
            this.toast.error('Error inesperado al actualizar la atracción.');
          }

          console.error('Error al actualizar atracción:', err);
        },
      });
  }
}
