import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/AuthService';

export const roleGuard = (rolesPermitidos: string[]): CanActivateFn => {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);
    const rol = auth.getUserRole();

    if (!rolesPermitidos.includes(rol)) {
      router.navigate(['/login']);
      return false;
    }

    return true;
  };
};
