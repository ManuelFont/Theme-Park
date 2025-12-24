import { Component, inject } from '@angular/core';
import { UserForm } from '../user-form/user-form';
import { ActivatedRoute, Router } from '@angular/router';
import { HotToastService } from '@ngxpert/hot-toast';
import { finalize } from 'rxjs';
import { UsuariosService } from '../../../../../services/UsuariosService';
import ActualizarUsuarioRequest from '../../../../../models/Usuario/ActualizarUsuarioRequest.model';
import Usuario from '../../../../../models/Usuario/Usuario.model';
import { UsuarioFormOutput } from '../../../../../models/Usuario/UsuarioFormOutput.model';

@Component({
  selector: 'app-editar-usuario',
  imports: [UserForm],
  templateUrl: './editar-usuario.html',
  styleUrl: './editar-usuario.css',
})
export class EditarUsuario {
  private readonly route = inject(ActivatedRoute);
  private readonly usuarioService = inject(UsuariosService);
  private readonly toast = inject(HotToastService);
  private readonly router = inject(Router);

  usuario: Usuario | null = null;
  id!: string;
  loading = false;

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id')!;
    this.cargarUsuario();
  }

  private cargarUsuario() {
    this.loading = true;
    this.usuarioService
      .getById(this.id)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (data) => {
          console.log('Usuario recibido:', data);
          this.usuario = data;
        },
        error: () => this.toast.error('Error al cargar el usuario'),
      });
  }

  submit(formValue: UsuarioFormOutput) {
    if (this.loading || !this.usuario) return;
    this.loading = true;

    const body: ActualizarUsuarioRequest = {
      nombre: formValue.nombre.trim(),
      apellido: formValue.apellido.trim(),
      email: formValue.email.trim(),
      contrasenia: formValue.contrasenia?.trim() || null,
      fechaNacimiento:
        this.usuario.tipoUsuario === 'Visitante' && formValue.fechaNacimiento
          ? new Date(formValue.fechaNacimiento).toISOString()
          : null,
      nivelMembresia: this.usuario.tipoUsuario === 'Visitante' ? formValue.nivelMembresia : null,
    };

    if (formValue.contrasenia && formValue.contrasenia.trim() !== '') {
      body.contrasenia = formValue.contrasenia;
    }

    if (this.usuario.tipoUsuario === 'Visitante') {
      body.fechaNacimiento = formValue.fechaNacimiento
        ? new Date(formValue.fechaNacimiento).toISOString()
        : null;

      body.nivelMembresia = formValue.nivelMembresia
        ? (formValue.nivelMembresia as 'Estandar' | 'Premium' | 'Vip')
        : null;
    }

    this.usuarioService
      .update(this.id, body)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: () => {
          this.toast.success('Usuario actualizado correctamente');
          this.router.navigate(['/admin/usuarios']);
        },
        error: (err) => {
          if (err.status === 409) {
            this.toast.error('El email ya está en uso por otro usuario.');
          } else if (err.status === 400) {
            this.toast.error('Datos inválidos. Revisá los campos.');
          } else if (err.status === 404) {
            this.toast.error('Usuario no encontrado.');
          } else {
            this.toast.error('Error inesperado al actualizar el usuario.');
          }

          console.error('Error al actualizar usuario:', err);
        },
      });
  }
}
