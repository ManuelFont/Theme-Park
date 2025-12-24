import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HotToastService } from '@ngxpert/hot-toast';
import VisitanteInfo from '../../../../models/VisitanteInfo.model';
import { VisitanteService } from '../../../../services/VisitanteService';

@Component({
  selector: 'app-perfil',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './perfil.html',
  styleUrl: './perfil.css',
})
export class Perfil implements OnInit {
  visitante: VisitanteInfo | null = null;

  private readonly visitanteService = inject(VisitanteService);
  private readonly toast = inject(HotToastService);

  ngOnInit() {
    this.cargarPerfil();
  }

  cargarPerfil() {
    this.visitanteService.getMiInformacion().subscribe({
      next: (data) => (this.visitante = data),
      error: () => this.toast.error('Error al cargar perfil'),
    });
  }
}
