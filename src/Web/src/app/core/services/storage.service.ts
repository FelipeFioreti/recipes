import {Injectable} from '@angular/core';

@Injectable({providedIn: 'root'})
export class StorageService {
    setStorageItem(key: string, value: unknown): void {
        localStorage.setItem(key, JSON.stringify(value));
    }

    getStorageItem<T>(key: string): T | null {

        const item = localStorage.getItem(key);

        if (!item) {
            return null;
        }

        return JSON.parse(item) as T;
    }

    removeStorageItem(key: string): void {
        localStorage.removeItem(key);
    }

    clear(): void {
        localStorage.clear();
    }

    hasItem(key: string): boolean {
        return !!localStorage.getItem(key);
    }
}

