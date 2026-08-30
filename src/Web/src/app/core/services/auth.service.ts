import {HttpClient, HttpContext} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Router} from '@angular/router';
import {catchError, EMPTY, finalize, Observable, shareReplay, tap} from 'rxjs';
import {environment} from '../../../environments/environment';
import {SKIP_AUTH_REFRESH, SKIP_ERROR_TOAST} from '../http/http-context.tokens';
import {REFRESH_TOKEN_KEY, TOKEN_KEY} from '../constants/keys.constants'
import {AuthResponse} from "../models/auth.model";
import {StorageService} from "./storage.service";
import {jwtDecode} from "jwt-decode";
import {AppJwtPayload} from "../models/jwt-payload.model";
import {Role} from "../enums/role";

@Injectable({providedIn: 'root'})
export class AuthService {

    constructor(
        private storageService: StorageService,
        private http: HttpClient,
        private router: Router,
    ) {
    }

    private readonly accountUrl = `${environment.apiUrl}/account`;
    private refreshSessionRequest$?: Observable<AuthResponse>;

    login(payload: {
        email: string,
        password: string,
    }): Observable<AuthResponse> {
        return this.http
            .post<AuthResponse>(`${this.accountUrl}/login`, payload,
                {
                    context: new HttpContext()
                        .set(SKIP_ERROR_TOAST, true)
                        .set(SKIP_AUTH_REFRESH, true)
                })
            .pipe(tap((response) => {
                this.storeSession(response);
            }));
    }

    register(payload: {
        name: string,
        email: string,
        password: string,
    }): Observable<void> {
        return this.http
            .post<void>(`${this.accountUrl}/register`, payload);
    }

    logout(): void {
        const refreshToken = this.getRefreshToken();

        if (!refreshToken) {
            this.endSession();
            return;
        }

        this.http
            .post<void>(`${this.accountUrl}/logout`, {refreshToken}, {
                context: new HttpContext()
                    .set(SKIP_ERROR_TOAST, true)
                    .set(SKIP_AUTH_REFRESH, true)
            })
            .pipe(
                catchError(() => EMPTY),
                finalize(() => this.endSession())
            )
            .subscribe();
    }

    refreshSession(): Observable<AuthResponse> {
        if (this.refreshSessionRequest$) {
            return this.refreshSessionRequest$;
        }

        const refreshToken = this.getRefreshToken();

        this.refreshSessionRequest$ = this.http
            .post<AuthResponse>(`${this.accountUrl}/refresh`, {refreshToken}, {
                context: new HttpContext()
                    .set(SKIP_ERROR_TOAST, true)
                    .set(SKIP_AUTH_REFRESH, true)
            })
            .pipe(
                tap((response) => {
                    this.storeSession(response);
                }),
                finalize(() => {
                    this.refreshSessionRequest$ = undefined;
                }),
                shareReplay({bufferSize: 1, refCount: false})
            );

        return this.refreshSessionRequest$;
    }

    expireSession(): void {
        this.endSession();
    }

    clearSession(): void {
        this.storageService.removeStorageItem(TOKEN_KEY);
        this.storageService.removeStorageItem(REFRESH_TOKEN_KEY);
    }

    private endSession(): void {
        this.clearSession();
        void this.router.navigateByUrl('/auth/login');
    }

    private storeSession(response: AuthResponse): void {
        this.storageService.setStorageItem(TOKEN_KEY, response.accessToken);
        this.storageService.setStorageItem(REFRESH_TOKEN_KEY, response.refreshToken);
    }

    private getToken(): string | null {
        return this.storageService.getStorageItem(TOKEN_KEY);
    }

    private getRefreshToken(): string | null {
        return this.storageService.getStorageItem(REFRESH_TOKEN_KEY);
    }

    hasRefreshToken(): boolean {
        return !!this.storageService.getStorageItem(REFRESH_TOKEN_KEY);
    }

    isAuthenticated(): boolean {

        const jwtPayload = this.getJwtPayload();

        if (!jwtPayload?.exp) {
            return false;
        }

        return jwtPayload.exp * 1000 > Date.now();
    }

    private getJwtPayload(): AppJwtPayload | null {

        const token = this.getToken();

        if (!token) {
            return null;
        }

        try {
            return jwtDecode<AppJwtPayload>(token);
        } catch {
            return null;
        }
    }

    getRoles(): Role[] {

        const jwtPayload = this.getJwtPayload();

        if (!jwtPayload?.role) {
            return [];
        }

        return Array.isArray(jwtPayload.role)
            ? jwtPayload.role
            : [jwtPayload.role];
    }

    getDisplayName(): string | null {

        const jwtPayload = this.getJwtPayload();

        return jwtPayload?.unique_name ?? null;
    }


    isAdmin(): boolean {
        return this.getRoles().includes(Role.ADMIN);
    }
}
