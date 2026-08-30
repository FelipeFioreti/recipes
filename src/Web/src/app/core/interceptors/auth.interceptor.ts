import {HttpErrorResponse, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {inject, Injectable, Injector} from '@angular/core';
import {catchError, switchMap, throwError} from 'rxjs';
import {StorageService} from "../services/storage.service";
import {REFRESH_TOKEN_KEY, TOKEN_KEY} from "../constants/keys.constants";
import {AuthService} from "../services/auth.service";
import {SKIP_AUTH_REFRESH} from "../http/http-context.tokens";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

    private readonly storageService = inject(StorageService);
    private readonly authService = inject(AuthService);
    private readonly injector = inject(Injector);

    intercept(req: HttpRequest<any>, next: HttpHandler) {
        const token = this.storageService.getStorageItem(TOKEN_KEY);
        if (token) {
            req = req.clone({
                setHeaders: {Authorization: `Bearer ${token}`}
            });
        }

        return next.handle(req).pipe(
            catchError((error: HttpErrorResponse) => {
                const refreshToken = this.storageService.getStorageItem(REFRESH_TOKEN_KEY);

                if (error.status !== 401 || req.context.get(SKIP_AUTH_REFRESH) || !refreshToken) {
                    return throwError(() => error);
                }

                return this.authService.refreshSession().pipe(
                    switchMap((response) => {
                        const retryRequest = req.clone({
                            setHeaders: {Authorization: `Bearer ${response.accessToken}`}
                        });

                        return next.handle(retryRequest);
                    }),
                    catchError((refreshError: HttpErrorResponse) => {
                        this.authService.expireSession();
                        return throwError(() => refreshError);
                    })
                );
            })
        );
    }
}
