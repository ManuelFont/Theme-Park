import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { ZardButtonComponent } from '@shared/components/button/button.component';
import { ZardCardComponent } from '@shared/components/card/card.component';
import { ZardIconComponent } from '@shared/components/icon/icon.component';
import { ZardInputDirective } from '@shared/components/input/input.directive';

import { ArrowLeft, CirclePlus } from 'lucide-angular';
import { HotToastService } from '@ngxpert/hot-toast';

import Atraccion from '../../../../../models/Atraccion/Atraccion.model';
import CrearEventoRequest from '../../../../../models/Evento/CrearEventoRequest.model';
import { AtraccionesService } from '../../../../../services/AtraccionesService';
import { EventosService } from '../../../../../services/EventosService';

import { finalize, switchMap } from 'rxjs';

@Component({
  selector: 'app-crear-evento',
  standalone: true,
  templateUrl: './crear-evento.html',
  styleUrl: './crear-evento.css',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    ZardButtonComponent,
    ZardCardComponent,
    ZardInputDirective,
    ZardIconComponent,
  ],
})
export class CrearEvento implements OnInit {
  CirclePlus = CirclePlus;
  ArrowLeft = ArrowLeft;

  form!: FormGroup;
  loading = false;

  atracciones: Atraccion[] = [];
  seleccionadas: string[] = [];

  private readonly fb = inject(FormBuilder);
  private readonly toast = inject(HotToastService);
  private readonly router = inject(Router);
  private readonly atraccionesService = inject(AtraccionesService);
  private readonly eventosService = inject(EventosService);

  ngOnInit(): void {
    this.form = this.fb.group({
      nombre: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      fecha: ['', Validators.required],
      hora: ['', Validators.required],
      aforo: [1, [Validators.required, Validators.min(1)]],
      costoAdicional: [0, [Validators.required, Validators.min(0)]],
    });

    this.cargarAtracciones();
  }

  get f() {
    return this.form.controls;
  }

  private cargarAtracciones(): void {
    this.atraccionesService.getAll().subscribe({
      next: (data) => (this.atracciones = data),
      error: () => this.toast.error('Error al cargar atracciones'),
    });
  }

  estaSeleccionada(id: string): boolean {
    return this.seleccionadas.includes(id);
  }

  toggleAtraccion(id: string): void {
    this.seleccionadas = this.estaSeleccionada(id)
      ? this.seleccionadas.filter((x) => x !== id)
      : [...this.seleccionadas, id];
  }

  onSubmit(): void {
    if (this.loading) return;
    if (!this.isFormValid()) return;

    this.loading = true;

    const body = this.buildRequestBody();

    this.eventosService
      .create(body)
      .pipe(
        switchMap((eventoCreado) =>
          this.eventosService.addMultipleAttractions(eventoCreado.id, this.seleccionadas)
        ),
        finalize(() => (this.loading = false))
      )
      .subscribe({
        next: () => this.handleSuccess(),
        error: (err) => this.handleError(err),
      });
  }

  private isFormValid(): boolean {
    if (this.form.invalid) {
      this.toast.info('Debes completar todos los campos correctamente');
      this.form.markAllAsTouched();
      return false;
    }

    if (this.seleccionadas.length === 0) {
      this.toast.info('Debes seleccionar al menos una atracción');
      return false;
    }

    return true;
  }

  private buildRequestBody(): CrearEventoRequest {
    const { nombre, fecha, hora, aforo, costoAdicional } = this.form.value;

    const horaFormateada = hora.length === 5 ? `${hora}:00` : hora;
    const fechaFormateada = `${fecha}T${hora.substring(0, 5)}`;

    const body: CrearEventoRequest = {
      nombre: nombre.trim(),
      fecha: fechaFormateada,
      hora: horaFormateada,
      aforo: Number(aforo),
      costoAdicional: Number(costoAdicional),
    };

    return body;
  }

  private handleSuccess(): void {
    this.toast.success('Evento creado exitosamente');
    this.router.navigate(['/admin/eventos']);
  }

  private handleError(err: any): void {
    console.error('Error creating event:', err);
    this.toast.error(err.error?.mensaje ?? 'Error al crear evento.');
  }
}
