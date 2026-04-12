import { Injectable, computed, signal } from '@angular/core';
import { STORAGE_KEYS } from '../constants/storage-keys';
import { AuthApiResponse, UserSession, mapSessionFromAuth } from '../models/user-session.model';

@Injectable({ providedIn: 'root' })
export class AuthStore {
  private readonly sessionState = signal<UserSession | null>(this.restoreSession());

  readonly session = computed(() => this.sessionState());
  readonly isAuthenticated = computed(() => !!this.sessionState()?.token);
  readonly isAdmin = computed(() => this.sessionState()?.role === 'ADMIN');
  readonly displayName = computed(() => this.sessionState()?.name ?? 'Convidado');

  setSession(response: AuthApiResponse): UserSession {
    const session = mapSessionFromAuth(response);

    this.sessionState.set(session);
    localStorage.setItem(STORAGE_KEYS.session, JSON.stringify(session));

    return session;
  }

  clear(): void {
    this.sessionState.set(null);
    localStorage.removeItem(STORAGE_KEYS.session);
  }

  private restoreSession(): UserSession | null {
    const savedSession = localStorage.getItem(STORAGE_KEYS.session);

    if (!savedSession) {
      return null;
    }

    try {
      const parsedSession = JSON.parse(savedSession) as UserSession;

      if (parsedSession.expiresAt && parsedSession.expiresAt <= Date.now()) {
        localStorage.removeItem(STORAGE_KEYS.session);
        return null;
      }

      return parsedSession;
    } catch {
      localStorage.removeItem(STORAGE_KEYS.session);
      return null;
    }
  }
}

