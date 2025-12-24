import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { CommonModule } from '@angular/common';

import { AuthService } from '../../../services/AuthService';
import CrearUsuarioRequest from '../../../models/Usuario/CrearUsuarioRequest.model';

import { ZardCardComponent } from '@shared/components/card/card.component';
import { ZardButtonComponent } from '@shared/components/button/button.component';
import { ZardSelectComponent } from '@shared/components/select/select.component';
import { ZardSelectItemComponent } from '@shared/components/select/select-item.component';
import { ZardIconComponent } from '@shared/components/icon/icon.component';
import { LucideAngularModule, Eye, EyeOff } from 'lucide-angular';
import { ZardInputDirective } from '@shared/components/input/input.directive';
import { validarFechaNacimiento } from '@shared/utils/validador-fecha';
import { HotToastService } from '@ngxpert/hot-toast';

@Component({
  selector: 'app-register',
  standalone: true,
  templateUrl: './register.html',
  styleUrls: ['./register.css'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    ZardInputDirective,
    ZardCardComponent,
    ZardButtonComponent,
    ZardIconComponent,
    LucideAngularModule,
  ],
})
export class Register {
  form: FormGroup;
  loading = false;

  iconEye = Eye;
  iconEyeOff = EyeOff;
  hidePassword = true;
  hideConfirm = true;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private toast: HotToastService
  ) {
    this.form = this.fb.group(
      {
        nombre: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
        apellido: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
        email: ['', [Validators.required, Validators.email]],
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(6),
            Validators.pattern(/^(?=.*[!@#$%^&*()_+\-=[\]{};':"\\|,.<>/?]).+$/),
          ],
        ],

        confirmPassword: ['', Validators.required],
        tipoUsuario: ['Visitante', Validators.required],
        fechaNacimiento: [null],
        membresia: ['Estandar'],
      },
      { validator: this.matchPasswords }
    );
  }

  get f() {
    return this.form.controls;
  }

  matchPasswords(group: FormGroup) {
    return group.get('password')?.value === group.get('confirmPassword')?.value
      ? null
      : { mismatch: true };
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.toast.show('Completá todos los campos.');
      return;
    }

    const { tipoUsuario, fechaNacimiento, membresia } = this.form.value;

    if (tipoUsuario === 'Visitante') {
      if (!membresia) {
        this.toast.show('Debes seleccionar una membresía si eres visitante.');
        return;
      }

      const errorFecha = validarFechaNacimiento(fechaNacimiento);
      if (errorFecha) {
        this.toast.show(errorFecha);
        return;
      }
    }

    if (this.loading) return;
    this.loading = true;

    const body: CrearUsuarioRequest = {
      nombre: this.f['nombre'].value.trim(),
      apellido: this.f['apellido'].value.trim(),
      email: this.f['email'].value.trim(),
      contrasenia: this.f['password'].value,
      tipoUsuario,
      fechaNacimiento:
        tipoUsuario === 'Visitante' && fechaNacimiento
          ? new Date(fechaNacimiento).toISOString().substring(0, 16)
          : null,

      nivelMembresia: tipoUsuario === 'Visitante' ? this.f['membresia'].value : null,
    };

    console.log(body);

    this.auth
      .register(body)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: () => {
          this.toast.success('Cuenta creada exitosamente.');
          this.router.navigate(['/login']);
        },
        error: (err) => {
          this.toast.error(err.error ?? 'Error al registrar usuario');
          console.error(err);
        },
      });
  }
}
