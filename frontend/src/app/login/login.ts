import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../../services/AuthService';
import LoginRequest from '../../../models/Auth/LoginRequest.model';
import LoginResponse from '../../../models/Auth/LoginResponse.model';

import { CommonModule } from '@angular/common';
import { ZardCardComponent } from '@shared/components/card/card.component';
import { ZardButtonComponent } from '@shared/components/button/button.component';
import { ZardIconComponent } from '@shared/components/icon/icon.component';
import { LucideAngularModule, Eye, EyeOff } from 'lucide-angular';
import { HotToastService } from '@ngxpert/hot-toast';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    ZardCardComponent,
    ZardButtonComponent,
    ZardIconComponent,
    LucideAngularModule,
  ],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  iconEye = Eye;
  iconEyeOff = EyeOff;

  email = '';
  password = '';
  hidePassword = true;
  loading = false;

  constructor(private auth: AuthService, private router: Router, private toast: HotToastService) {}

  togglePassword() {
    this.hidePassword = !this.hidePassword;
  }

  submit(form: NgForm) {
    if (form.invalid) {
      form.control.markAllAsTouched();
      this.toast.info('Debes completar todos los campos');
      return;
    }

    if (this.loading) return;
    this.loading = true;

    const body: LoginRequest = {
      email: this.email,
      contrasenia: this.password,
    };

    this.auth
      .login(body)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (res: LoginResponse) => {
          this.toast.success('Has iniciado sesión exitosamente.');
          this.auth.saveSession(res.token, res.expiraEn);
          const rol = this.auth.getUserRole();
          switch (rol) {
            case 'Administrador':
              this.router.navigate(['/admin/inicio']);
              break;
            case 'Operador':
              this.router.navigate(['/operador/inicio']);
              break;
            default:
              this.router.navigate(['/visitante/perfil']);
              break;
          }
        },
        error: (err) => {
          this.toast.error(err.error.mensaje ?? 'Error al iniciar sesión');
        },
      });
  }
}
