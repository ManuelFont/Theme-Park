import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ZardButtonComponent } from '@shared/components/button/button.component';
import { ZardIconComponent } from '@shared/components/icon/icon.component';
import { LucideAngularModule, SquarePen, TrashIcon } from 'lucide-angular';
import Atraccion from '../../../../models/Atraccion/Atraccion.model';
import { ZardTableComponent } from '@shared/components/table/table.component';
import { AtraccionesService } from '../../../../services/AtraccionesService';
import { HotToastService } from '@ngxpert/hot-toast';
import { ZardAlertDialogService } from '@shared/components/alert-dialog/alert-dialog.service';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ListaAtracciones } from '../../lista-atracciones/lista-atracciones';

@Component({
  selector: 'app-atracciones',
  imports: [CommonModule, ListaAtracciones],
  templateUrl: './atracciones.html',
  styleUrl: './atracciones.css',
})
export class Atracciones {
  TrashIcon = TrashIcon;
  EditIcon = SquarePen;

  private atraccionService = inject(AtraccionesService);
  private readonly toast = inject(HotToastService);
  private readonly alertDialogService = inject(ZardAlertDialogService);
  private readonly router = inject(Router);

  atracciones: Atraccion[] = [];

  ngOnInit() {
    this.cargarUsuarios();
  }

  cargarUsuarios() {
    this.atraccionService.getAll().subscribe({
      next: (data) => (this.atracciones = data),
      error: () => this.toast.error('Error al cargar atracciones'),
    });
  }

  crearAtraccion() {
    this.router.navigate(['admin/atracciones/crear']);
  }

  async eliminarAtraccion(id: string) {
    const dialogRef = this.alertDialogService.confirm({
      zTitle: 'Eliminar atracción',
      zDescription: 'Esta accion no se puede revertir. ¿Deseas continuar?',
      zCancelText: 'Cancelar',
      zOkText: 'Eliminar',
    });

    const confirmed = await firstValueFrom(dialogRef.afterClosed());
    if (!confirmed) return;
    this.atraccionService.delete(id).subscribe({
      next: () => {
        this.atracciones = this.atracciones.filter((a) => a.id !== id);
      },
      error: (err) => {
        this.toast.error(err.error.message);
      },
    });
  }

  editarAtraccion(atraccion: Atraccion) {
    this.router.navigate(['admin/atracciones/editar', atraccion.id]);
  }
}
