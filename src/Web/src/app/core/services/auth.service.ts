import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { map, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SKIP_ERROR_TOAST } from '../http/http-context.tokens';
import { UserSession, AuthApiResponse } from '../models/user-session.model';
import { AuthStore } from './auth.store';

export interface SignInPayload {
  email: string;
  password: string;
}

export interface SignUpPayload {
  name: string;
  email: string;
  password: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly authStore = inject(AuthStore);
  private readonly accountUrl = `${environment.apiUrl}/account`;

  signIn(payload: SignInPayload): Observable<UserSession> {
    return this.http
      .post<AuthApiResponse>(`${this.accountUrl}/authenticate`, payload, {
        context: new HttpContext().set(SKIP_ERROR_TOAST, true)
      })
      .pipe(map((response) => this.authStore.setSession(response)));
  }

  signUp(payload: SignUpPayload): Observable<void> {
    return this.http
      .post(`${this.accountUrl}/register`, payload, {
        context: new HttpContext().set(SKIP_ERROR_TOAST, true)
      })
      .pipe(map(() => void 0));
  }

  signOut(): void {
    this.authStore.clear();
    void this.router.navigateByUrl('/auth/sign-in');
  }
}

