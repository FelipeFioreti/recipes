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

        try {
            return JSON.parse(item) as T;
        } catch {
            return item as unknown as T;
        }
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

