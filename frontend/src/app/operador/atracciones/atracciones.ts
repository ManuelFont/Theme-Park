import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AtraccionesService } from '../../../../services/AtraccionesService';
import { inject } from '@angular/core';
import Atraccion from '../../../../models/Atraccion/Atraccion.model';
import { ListaAtracciones } from '../../lista-atracciones/lista-atracciones';

@Component({
  selector: 'app-atracciones',
  imports: [ListaAtracciones],
  templateUrl: './atracciones.html',
  styleUrl: './atracciones.css',
})
export class Atracciones {
  atracciones: Atraccion[] = [];
  private router = inject(Router);
  private atraccionService = inject(AtraccionesService);

  ngOnInit() {
    this.atraccionService.getAll().subscribe({
      next: (data) => (this.atracciones = data),
      error: () => console.error('Error al cargar atracciones'),
    });
  }

  irADashboard(atraccion: Atraccion) {
    this.router.navigate(['operador/atraccion', atraccion.id]);
  }
}
