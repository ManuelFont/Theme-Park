import { Component, inject, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { LucideAngularModule, Trash2 } from 'lucide-angular';

import Evento from '../../../../models/Evento/Evento.model';
import { EventosService } from '../../../../services/EventosService';

import { ZardButtonComponent } from '@shared/components/button/button.component';
import { ZardIconComponent } from '@shared/components/icon/icon.component';
import {
  ZardTableBodyComponent,
  ZardTableCellComponent,
  ZardTableComponent,
  ZardTableHeadComponent,
  ZardTableRowComponent,
} from '@shared/components/table/table.component';

import { HotToastService } from '@ngxpert/hot-toast';
import { ZardAlertDialogService } from '@shared/components/alert-dialog/alert-dialog.service';
import { firstValueFrom } from 'rxjs';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-eventos',
  standalone: true,
  templateUrl: './eventos.html',
  styleUrl: './eventos.css',
  imports: [
    CommonModule,
    DatePipe,
    LucideAngularModule,
    ZardButtonComponent,
    ZardIconComponent,
    ZardTableComponent,
    ZardTableBodyComponent,
    ZardTableCellComponent,
    ZardTableHeadComponent,
    ZardTableRowComponent,
  ],
})
export class Eventos implements OnInit {
  TrashIcon = Trash2;
  eventos: Evento[] = [];

  private readonly eventosService = inject(EventosService);
  private readonly toast = inject(HotToastService);
  private readonly alertDialogService = inject(ZardAlertDialogService);
  private readonly router = inject(Router);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.eventosService.getAll().subscribe({
      next: (data) => (this.eventos = data),
      error: () => this.toast.error('Error cargando eventos'),
    });
  }

  crearEvento() {
    this.router.navigate(['admin/eventos/crear']);
  }

  async eliminar(id: string): Promise<void> {
    const dialogRef = this.alertDialogService.confirm({
      zTitle: 'Eliminar evento',
      zDescription: 'Esta acción no se puede revertir. ¿Deseas continuar?',
      zCancelText: 'Cancelar',
      zOkText: 'Eliminar',
    });

    const confirmed = await firstValueFrom(dialogRef.afterClosed());
    if (!confirmed) return;

    this.eventosService.delete(id).subscribe({
      next: () => {
        this.load();
        this.toast.success('Evento eliminado exitosamente');
      },
      error: () => this.toast.error('Error al eliminar el evento'),
    });
  }
}
