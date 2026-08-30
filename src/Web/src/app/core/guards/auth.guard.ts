import {inject, Injectable} from "@angular/core";
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from "@angular/router";
import {AuthService} from "../services/auth.service";
import {REFRESH_TOKEN_KEY} from "../constants/keys.constants";
import {SKIP_AUTH_REFRESH} from "../http/http-context.tokens";
import {catchError, map, Observable, of, switchMap, throwError} from "rxjs";
import {HttpErrorResponse} from "@angular/common/http";
import {StorageService} from "../services/storage.service";

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {

  private readonly authService = inject(AuthService);
  private readonly storageService = inject(StorageService);
  private readonly router = inject(Router);

  canActivate(
      route: ActivatedRouteSnapshot,
      state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> {
    if (this.authService.isAuthenticated()) {
      return of(true);
    }

    if (this.authService.hasRefreshToken()) {
      return this.authService.refreshSession().pipe(
          map(() => true),
          catchError(() => {
            this.authService.expireSession();
            return of(this.router.createUrlTree(['/auth/login']));
          })
      );
    }

    return of(this.router.createUrlTree(['/auth/login']));
  }
}