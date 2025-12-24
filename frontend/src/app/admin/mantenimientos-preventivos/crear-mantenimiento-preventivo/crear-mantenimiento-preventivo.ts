import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { HotToastService } from '@ngxpert/hot-toast';
import { ZardButtonComponent } from '@shared/components/button/button.component';
import { ZardCardComponent } from '@shared/components/card/card.component';
import { ZardIconComponent } from '@shared/components/icon/icon.component';
import { ZardInputDirective } from '@shared/components/input/input.directive';
import { ArrowLeft } from 'lucide-angular';
import Atraccion from '../../../../../models/Atraccion/Atraccion.model';
import { AtraccionesService } from '../../../../../services/AtraccionesService';
import { Router } from '@angular/router';
import { MantenimientosPreventivosService } from '../../../../../services/MantenimientosPreventivosService';
import CrearMantenimientoPreventivoRequest from '../../../../../models/MantenimientoPreventivo/CrearMantenimientoPreventivoRequest.model';

@Component({
  selector: 'app-crear-mantenimiento-preventivo',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ZardButtonComponent,
    ZardCardComponent,
    ZardIconComponent,
  ],
  templateUrl: './crear-mantenimiento-preventivo.html',
  styleUrl: './crear-mantenimiento-preventivo.css',
})
export class CrearMantenimientoPreventivo {
  ArrowLeft = ArrowLeft;

  form = inject(FormBuilder).group({
    atraccionId: ['', Validators.required],
    descripcion: ['', [Validators.required, Validators.minLength(5)]],
    fecha: ['', Validators.required],
    horaInicio: ['', Validators.required],
    horaFin: ['', Validators.required],
  });

  loading = false;
  atracciones: Atraccion[] = [];

  private readonly toast = inject(HotToastService);
  private readonly router = inject(Router);
  private readonly atraccionesService = inject(AtraccionesService);
  private readonly mantenimientosService = inject(MantenimientosPreventivosService);

  ngOnInit(): void {
    this.cargarAtracciones();
  }

  cargarAtracciones() {
    this.atraccionesService.getAll().subscribe({
      next: (data) => (this.atracciones = data),
      error: () => this.toast.error('Error al cargar atracciones'),
    });
  }

  volver() {
    this.router.navigate(['/admin/mantenimientos-preventivos']);
  }

  onSubmit() {
    if (this.form.invalid || this.loading) {
      this.toast.info('Completa todos los campos');
      this.form.markAllAsTouched();
      return;
    }

    const { fecha, horaInicio, horaFin, descripcion, atraccionId } = this.form.value;

    const fechaInicio = `${fecha}T${horaInicio}`;
    const fechaFin = `${fecha}T${horaFin}`;

    if (new Date(fechaFin) <= new Date(fechaInicio)) {
      this.toast.error('La hora fin debe ser mayor a la hora de inicio');
      return;
    }

    this.loading = true;

    const body: CrearMantenimientoPreventivoRequest = {
      atraccionId: atraccionId!,
      descripcion: descripcion!,
      fechaInicio: fechaInicio!,
      fechaFin: fechaFin!,
    };

    console.log(body);

    this.mantenimientosService.create(body).subscribe({
      next: () => {
        this.toast.success('Mantenimiento creado correctamente');
        this.router.navigate(['/admin/mantenimientos-preventivos']);
      },
      error: (err) => this.toast.error(err.error?.mensaje ?? 'Error al crear mantenimiento'),
      complete: () => (this.loading = false),
    });
  }
}
