import { Component } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/AuthService';
import { ZardIconComponent } from '@shared/components/icon/icon.component';
import { ZardButtonComponent } from '@shared/components/button/button.component';

import {
  House,
  Users,
  BarChart3,
  Ticket,
  Settings,
  Gift,
  Wrench,
  Award,
  History,
  LogOut,
  FerrisWheel,
  Trophy,
  User,
  Package,
  TrendingUp,
  WrenchIcon,
} from 'lucide-angular';
import { iconVariants } from '@shared/components/icon/icon.variants';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, ZardIconComponent, ZardButtonComponent],
  templateUrl: './layout.html',
})
export class LayoutComponent {
  userRole: string;

  constructor(private auth: AuthService, private router: Router) {
    this.userRole = this.auth.getUserRole();
  }

  private menuByRole: Record<string, any[]> = {
    Administrador: [
      { label: 'Inicio', icon: House, path: '/admin/inicio' },
      { label: 'Usuarios', icon: Users, path: '/admin/usuarios' },
      { label: 'Atracciones', icon: FerrisWheel, path: '/admin/atracciones' },
      { label: 'Eventos especiales', icon: Ticket, path: '/admin/eventos' },
      {
        label: 'Mantenimientos preventivos',
        icon: WrenchIcon,
        path: '/admin/mantenimientos-preventivos',
      },
      { label: 'Estrategias', icon: Award, path: '/admin/estrategias' },
      { label: 'Reportes', icon: BarChart3, path: '/admin/reportes' },
      { label: 'Ranking diario', icon: Trophy, path: '/admin/ranking-diario' },
    ],

    Operador: [
      { label: 'Inicio', icon: House, path: '/operador/inicio' },
      { label: 'Atracciones', icon: FerrisWheel, path: '/operador/atracciones' },
      { label: 'Incidencias', icon: Wrench, path: '/operador/incidencias' },
    ],

    Visitante: [
      { label: 'Mi perfil', icon: User, path: '/visitante/perfil' },
      { label: 'Comprar tickets', icon: Ticket, path: '/visitante/comprar-tickets' },
      { label: 'Historial', icon: History, path: '/visitante/historial' },
      { label: 'Recompensas', icon: Gift, path: '/visitante/recompensas' },
      { label: 'Mis canjes', icon: Package, path: '/visitante/mis-canjes' },
      { label: 'Historial de puntos', icon: TrendingUp, path: '/visitante/historial-puntuacion' },
    ],
  };

  get links() {
    return this.menuByRole[this.userRole] ?? [];
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
