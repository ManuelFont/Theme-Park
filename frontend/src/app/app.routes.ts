import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Register } from './register/register';
import { LayoutComponent } from './layout/layout';
import { NotFoundComponent } from './not-found/not-found.component';
import { authGuard } from '../../guards/auth.guard';
import { guestGuard } from '../../guards/guest.guard';
import { roleGuard } from '../../guards/role.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  { path: 'login', component: Login, canActivate: [guestGuard] },
  { path: 'register', component: Register, canActivate: [guestGuard] },
  {
    path: 'inicio',
    loadComponent: () => import('./inicio/inicio').then((m) => m.Inicio),
    canActivate: [authGuard],
  },

  {
    path: 'admin',
    component: LayoutComponent,
    canActivate: [authGuard, roleGuard(['Administrador'])],
    children: [
      {
        path: 'inicio',
        loadComponent: () => import('./admin/admin-home/admin-home').then((m) => m.AdminHome),
      },
      {
        path: 'usuarios',
        loadComponent: () => import('./admin/usuarios/usuarios').then((m) => m.UsuariosComponent),
      },
      {
        path: 'usuarios/crear',
        loadComponent: () =>
          import('./admin/usuarios/crear-usuario/crear-usuario').then((m) => m.CrearUsuario),
      },
      {
        path: 'usuarios/editar/:id',
        loadComponent: () =>
          import('./admin/usuarios/editar-usuario/editar-usuario').then((m) => m.EditarUsuario),
      },
      {
        path: 'atracciones',
        loadComponent: () => import('./admin/atracciones/atracciones').then((m) => m.Atracciones),
      },
      {
        path: 'atracciones/crear',
        loadComponent: () =>
          import('./admin/atracciones/crear-atraccion/crear-atraccion').then(
            (m) => m.CrearAtraccion
          ),
      },
      {
        path: 'atracciones/editar/:id',
        loadComponent: () =>
          import('./admin/atracciones/editar-atraccion/editar-atraccion').then(
            (m) => m.EditarAtraccion
          ),
      },
      {
        path: 'eventos',
        loadComponent: () => import('./admin/eventos/eventos').then((m) => m.Eventos),
      },
      {
        path: 'eventos/crear',
        loadComponent: () =>
          import('./admin/eventos/crear-evento/crear-evento').then((m) => m.CrearEvento),
      },
      {
        path: 'mantenimientos-preventivos',
        loadComponent: () =>
          import('./admin/mantenimientos-preventivos/mantenimientos-preventivos').then(
            (m) => m.MantenimientoPreventivos
          ),
      },
      {
        path: 'mantenimientos-preventivos/crear',
        loadComponent: () =>
          import(
            './admin/mantenimientos-preventivos/crear-mantenimiento-preventivo/crear-mantenimiento-preventivo'
          ).then((m) => m.CrearMantenimientoPreventivo),
      },
      {
        path: 'estrategias',
        loadComponent: () =>
          import('./admin/estrategias/estrategias').then((m) => m.EstrategiasComponent),
      },
      {
        path: 'reportes',
        loadComponent: () =>
          import('./admin/reporte-uso-atracciones/reporte-uso-atracciones').then(
            (m) => m.ReporteUsoAtracciones
          ),
      },
      {
        path: 'ranking-diario',
        loadComponent: () =>
          import('./admin/ranking-diario/ranking-diario').then((m) => m.RankingDiario),
      },
    ],
  },
  {
    path: 'operador',
    component: LayoutComponent,
    canActivate: [authGuard, roleGuard(['Operador'])],
    children: [
      {
        path: 'inicio',
        loadComponent: () =>
          import('./operador/operador-home/operador-home').then((m) => m.OperadorHome),
      },
      {
        path: 'atracciones',
        loadComponent: () =>
          import('./operador/atracciones/atracciones').then((m) => m.Atracciones),
      },
      {
        path: 'atraccion/:id',
        loadComponent: () =>
          import('./operador/atraccion/atraccion').then((m) => m.AtraccionComponent),
      },
      {
        path: 'atraccion/:id/acceso',
        loadComponent: () => import('./operador/acceso/acceso').then((m) => m.AccesoComponent),
      },
      {
        path: 'atraccion/:id/egreso',
        loadComponent: () => import('./operador/egreso/egreso').then((m) => m.Egreso),
      },
      {
        path: 'atraccion/:id/incidencias',
        loadComponent: () =>
          import('./operador/incidencias/incidencias').then((m) => m.IncidenciasComponent),
      },
    ],
  },
  {
    path: 'visitante',
    component: LayoutComponent,
    canActivate: [authGuard, roleGuard(['Visitante'])],
    children: [
      {
        path: '',
        redirectTo: 'perfil',
        pathMatch: 'full',
      },
      {
        path: 'perfil',
        loadComponent: () => import('./visitante/perfil/perfil').then((m) => m.Perfil),
      },
      {
        path: 'comprar-tickets',
        loadComponent: () =>
          import('./visitante/comprar-tickets/comprar-tickets').then((m) => m.ComprarTickets),
      },
      {
        path: 'historial',
        loadComponent: () => import('./visitante/historial/historial').then((m) => m.Historial),
      },
      {
        path: 'recompensas',
        loadComponent: () =>
          import('./visitante/recompensas/recompensas').then((m) => m.Recompensas),
      },
      {
        path: 'mis-canjes',
        loadComponent: () => import('./visitante/mis-canjes/mis-canjes').then((m) => m.MisCanjes),
      },
      {
        path: 'historial-puntuacion',
        loadComponent: () =>
          import('./visitante/historial-puntuacion/historial-puntuacion').then(
            (m) => m.HistorialPuntuacionComponent
          ),
      },
    ],
  },

  { path: '**', component: NotFoundComponent },
];
