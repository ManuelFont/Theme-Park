import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ZardButtonComponent } from '@shared/components/button/button.component';
import { ZardSelectComponent } from '@shared/components/select/select.component';
import { IncidenciasService } from '../../../../services/IncidenciasService';
import { HotToastService } from '@ngxpert/hot-toast';
import Incidencia from '../../../../models/Incidencia.model';
import { ZardSelectItemComponent } from '@shared/components/select/select-item.component';
import { ZardBadgeComponent } from '@shared/components/badge/badge.component';

@Component({
  selector: 'app-incidencias',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ZardButtonComponent,
    ZardSelectItemComponent,
    ZardSelectComponent,
    ZardBadgeComponent,
  ],
  templateUrl: './incidencias.html',
})
export class IncidenciasComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private toast = inject(HotToastService);
  private incidenciasService = inject(IncidenciasService);

  atraccionId!: string;
  cargando = true;

  incidencias: Incidencia[] = [];

  creando = false;
  descripcion = '';
  tipo: string = '';

  tipos = [
    { label: 'Fuera de servicio', value: 'FueraDeServicio' },
    { label: 'Mantenimiento', value: 'Mantenimiento' },
    { label: 'Rota', value: 'Rota' },
  ];

  ngOnInit(): void {
    this.atraccionId = this.route.snapshot.paramMap.get('id')!;
    this.cargar();
  }

  cargar() {
    this.cargando = true;
    this.incidenciasService
      .obtenerPorAtraccion(this.atraccionId)
      .pipe(this.toast.observe({ loading: 'Cargando...', success: 'Incidencias cargadas' }))
      .subscribe({
        next: (data) => {
          this.incidencias = data;
          this.cargando = false;
        },
        error: () => {
          this.toast.error('Error al cargar las incidencias');
          this.cargando = false;
        },
      });
  }

  toggleCrear() {
    this.creando = !this.creando;
    this.descripcion = '';
    this.tipo = '';
  }

  onDescripcionChange(event: any) {
    this.descripcion = event.target.value;
  }

  crearIncidencia() {
    if (!this.descripcion || !this.tipo) {
      this.toast.error('Todos los campos son obligatorios');
      console.log(this.descripcion + ' , ', this.tipo);
      return;
    }

    this.incidenciasService
      .crear({
        atraccionId: this.atraccionId,
        descripcion: this.descripcion,
        tipoIncidencia: this.tipo,
      })
      .pipe(this.toast.observe({ success: 'Incidencia creada', loading: 'Creando...' }))
      .subscribe(() => {
        this.toggleCrear();
        this.cargar();
      });
  }

  cerrar(i: Incidencia) {
    this.incidenciasService
      .cerrar(i.id)
      .pipe(this.toast.observe({ success: 'Incidencia cerrada' }))
      .subscribe(() => {
        this.cargar();
      });
  }
}
