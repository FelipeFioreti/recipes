import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStore } from '../services/auth.store';
import { NotificationService } from '../services/notification.service';

export const adminGuard: CanActivateFn = () => {
  const authStore = inject(AuthStore);
  const router = inject(Router);
  const notificationService = inject(NotificationService);

  if (authStore.isAdmin()) {
    return true;
  }

  notificationService.error('Acesso restrito', 'Somente administradores podem acessar a gestao de categorias.');
  return router.createUrlTree(['/app/dashboard']);
};

