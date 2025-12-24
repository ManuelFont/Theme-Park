import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ZardButtonComponent } from '@shared/components/button/button.component';
import { AuthService } from '../../../services/AuthService';

@Component({
  standalone: true,
  selector: 'app-not-found',
  imports: [CommonModule, ZardButtonComponent],
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.css'],
})
export class NotFoundComponent {
  constructor(private router: Router, private auth: AuthService) {}

  goBack(): void {
    window.history.back();
  }

  goHome(): void {
    const rol = this.auth.getUserRole();
    switch (rol) {
      case 'Administrador':
        this.router.navigate(['/admin/inicio']);
        break;
      case 'Operador':
        this.router.navigate(['/operador/inicio']);
        break;
      case 'Visitante':
        this.router.navigate(['/visitante/inicio']);
        break;
      default:
        this.router.navigate(['/login']);
        break;
    }
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['login']);
  }
}
