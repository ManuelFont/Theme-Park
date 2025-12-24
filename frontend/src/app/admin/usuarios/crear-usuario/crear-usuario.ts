import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../../../services/AuthService';
import { HotToastService } from '@ngxpert/hot-toast';
import CrearUsuarioRequest from '../../../../../models/Usuario/CrearUsuarioRequest.model';
import { finalize } from 'rxjs';
import { UserForm } from '../user-form/user-form';
import { UsuarioFormOutput } from '../../../../../models/Usuario/UsuarioFormOutput.model';

@Component({
  selector: 'app-crear-usuario',
  imports: [UserForm],
  templateUrl: './crear-usuario.html',
  styleUrl: './crear-usuario.css',
})
export class CrearUsuario {
  private readonly auth = inject(AuthService);
  private readonly toast = inject(HotToastService);
  private readonly router = inject(Router);
  loading = false;

  submit(formValue: UsuarioFormOutput) {
    if (this.loading) return;
    this.loading = true;

    const body: CrearUsuarioRequest = {
      nombre: formValue.nombre.trim(),
      apellido: formValue.apellido.trim(),
      email: formValue.email.trim(),
      contrasenia: formValue.contrasenia!,
      tipoUsuario: formValue.tipoUsuario!,
      fechaNacimiento:
        formValue.tipoUsuario === 'Visitante' && formValue.fechaNacimiento
          ? new Date(formValue.fechaNacimiento).toISOString().substring(0, 16)
          : null,
      nivelMembresia: formValue.tipoUsuario === 'Visitante' ? formValue.nivelMembresia : null,
    };

    this.auth
      .register(body)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: () => {
          this.toast.success('Cuenta creada exitosamente.');
          this.router.navigate(['/admin/usuarios']);
        },
        error: (err) => {
          this.toast.error(err.error.mensaje ?? 'Error al registrar usuario');
          console.error(err);
        },
      });
  }
}
