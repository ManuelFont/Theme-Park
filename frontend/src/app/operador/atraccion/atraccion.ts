import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AtraccionesService } from '../../../../services/AtraccionesService';
import { AccesoAtraccionService } from '../../../../services/AccesoAtraccionService';
import Atraccion from '../../../../models/Atraccion/Atraccion.model';
import { ZardButtonComponent } from '@shared/components/button/button.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-atraccion',
  imports: [ZardButtonComponent, CommonModule],
  templateUrl: './atraccion.html',
  styleUrl: './atraccion.css',
})
export class AtraccionComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private atraccionesService = inject(AtraccionesService);
  private accesoService = inject(AccesoAtraccionService);

  id = this.route.snapshot.paramMap.get('id');
  atraccion?: Atraccion;

  aforoActual: number = 0;
  cargando = true;

  ngOnInit() {
    if (!this.id) return;

    this.route.params.subscribe(() => {
      this.cargarDatos();
    });
  }

  cargarDatos() {
    if (!this.id) return;

    this.cargando = true;

    this.atraccionesService.getById(this.id).subscribe({
      next: (data: Atraccion) => {
        this.atraccion = data;

        this.cargarAforo();

        this.cargando = false;
      },
      error: () => {
        this.cargando = false;
        console.error('Error al cargar atracción');
      },
    });
  }

  cargarAforo() {
    if (!this.id) return;

    this.accesoService.obtenerAforo(this.id).subscribe({
      next: (n) => {
        this.aforoActual = n;
        console.log(n);
      },
      error: () => {
        console.error('No se pudo obtener el aforo actual.');
        this.aforoActual = 0;
      },
    });
  }

  irA(ruta: string) {
    this.router.navigate(['operador', 'atraccion', this.id, ruta]);
  }
}
