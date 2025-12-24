import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ZardButtonComponent } from '@shared/components/button/button.component';
import { ZardIconComponent } from '@shared/components/icon/icon.component';
import { HotToastService } from '@ngxpert/hot-toast';
import { LucideAngularModule, SquarePen, Trash2 } from 'lucide-angular';
import {
  ZardTableBodyComponent,
  ZardTableCellComponent,
  ZardTableComponent,
  ZardTableHeadComponent,
  ZardTableRowComponent,
} from '@shared/components/table/table.component';
import Usuario from '../../../../models/Usuario/Usuario.model';
import { UsuariosService } from '../../../../services/UsuariosService';
import { ZardAlertDialogService } from '@shared/components/alert-dialog/alert-dialog.service';
import { AuthService } from '../../../../services/AuthService';
import { firstValueFrom } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-usuarios',
  standalone: true,
  templateUrl: './usuarios.html',
  imports: [
    CommonModule,
    ZardButtonComponent,
    ZardIconComponent,
    LucideAngularModule,
    ZardTableComponent,
    ZardTableBodyComponent,
    ZardTableCellComponent,
    ZardTableHeadComponent,
    ZardTableRowComponent,
  ],
})
export class UsuariosComponent implements OnInit {
  TrashIcon = Trash2;
  EditIcon = SquarePen;
  usuarios: Usuario[] = [];

  private readonly usuarioService = inject(UsuariosService);
  private readonly auth = inject(AuthService);
  private readonly toast = inject(HotToastService);
  private readonly alertDialogService = inject(ZardAlertDialogService);
  private readonly router = inject(Router);

  ngOnInit() {
    this.cargarUsuarios();
  }

  cargarUsuarios() {
    this.usuarioService.getAll().subscribe({
      next: (data) => (this.usuarios = data),
      error: () => this.toast.error('Error al cargar usuarios'),
    });
  }

  async eliminarUsuario(id: string) {
    const currentUserEmail = this.auth.getCurrentUser();
    if (!currentUserEmail) return;

    if (currentUserEmail === this.usuarios.find((u) => u.id === id)?.email) {
      this.toast.error('No puedes eliminarte');
      return;
    }

    const dialogRef = this.alertDialogService.confirm({
      zTitle: 'Eliminar usuario',
      zDescription: 'Esta accion no se puede revertir. ¿Deseas continuar?',
      zCancelText: 'Cancelar',
      zOkText: 'Eliminar',
    });

    const confirmed = await firstValueFrom(dialogRef.afterClosed());
    if (!confirmed) return;

    this.usuarioService.delete(id).subscribe({
      next: () => {
        this.cargarUsuarios();
        this.toast.success('Usuario eliminado exisotasmente');
      },
      error: () => this.toast.error('Error al eliminar usuario'),
    });
  }

  editarUsuario(usuario: Usuario) {
    this.router.navigate(['admin/usuarios/editar', usuario.id]);
  }

  crearUsuario() {
    this.router.navigate(['admin/usuarios/crear']);
  }
}
