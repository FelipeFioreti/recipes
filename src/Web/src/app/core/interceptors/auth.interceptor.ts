import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthStore } from '../services/auth.store';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const session = inject(AuthStore).session();

  if (!session?.token || !request.url.startsWith('/api')) {
    return next(request);
  }

  return next(
    request.clone({
      setHeaders: {
        Authorization: `Bearer ${session.token}`
      }
    })
  );
};

