import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { SKIP_ERROR_TOAST } from '../http/http-context.tokens';
import { ApiErrorResponse } from '../models/api-error.model';
import { NotificationService } from '../services/notification.service';

function readValidationErrors(error: ApiErrorResponse): string | null {
  if (!error.errors) {
    return null;
  }

  const flattenedErrors = Object.values(error.errors).flat().filter(Boolean);

  return flattenedErrors.length ? flattenedErrors.join(' ') : null;
}

function extractMessage(error: HttpErrorResponse): string {
  const payload = error.error as ApiErrorResponse | string | null;

  if (typeof payload === 'string' && payload.trim()) {
    return payload;
  }

  if (payload && typeof payload === 'object') {
    return readValidationErrors(payload) ?? payload.message ?? 'Nao foi possivel concluir a operacao.';
  }

  if (error.status === 0) {
    return 'Nao foi possivel se conectar com a API.';
  }

  return 'Nao foi possivel concluir a operacao.';
}

export const errorInterceptor: HttpInterceptorFn = (request, next) => {
  const notificationService = inject(NotificationService);

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      if (!request.context.get(SKIP_ERROR_TOAST)) {
        notificationService.error('Operacao interrompida', extractMessage(error));
      }

      return throwError(() => error);
    })
  );
};
