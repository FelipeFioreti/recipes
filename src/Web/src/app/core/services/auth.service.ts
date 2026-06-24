import {HttpClient, HttpContext} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Router} from '@angular/router';
import {Observable, tap} from 'rxjs';
import {environment} from '../../../environments/environment';
import {SKIP_ERROR_TOAST} from '../http/http-context.tokens';
import {TOKEN_KEY} from '../constants/keys.constants'
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

    login(payload: {
        email: string,
        password: string,
    }): Observable<AuthResponse> {
        return this.http
            .post<AuthResponse>(`${this.accountUrl}/authenticate`, payload,
                {
                    context: new HttpContext().set(SKIP_ERROR_TOAST, true)
                })
            .pipe(tap((response) => {
                this.storageService.setStorageItem(TOKEN_KEY, response.token);
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
        this.storageService.removeStorageItem(TOKEN_KEY);

        void this.router.navigateByUrl('/auth/login');
    }

    getToken(): string | null {
        return this.storageService.getStorageItem(TOKEN_KEY);
    }

    isAuthenticated(): boolean {

        const jwtPayload = this.getJwtPayload();

        if (!jwtPayload?.exp) {
            return false;
        }

        return jwtPayload.exp * 1000 > Date.now();
    }

    getJwtPayload(): AppJwtPayload | null {

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

    getRole(): Role | null {

        const jwtPayload = this.getJwtPayload();

        return jwtPayload?.role ?? null;
    }

    getDisplayName(): string | null {

        const jwtPayload = this.getJwtPayload();

        return jwtPayload?.unique_name ?? null;
    }


    isAdmin(): boolean {
        return this.getRole() === Role.ADMIN;
    }
}

