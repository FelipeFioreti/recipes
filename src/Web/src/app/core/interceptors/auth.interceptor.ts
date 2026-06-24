import {HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {inject, Injectable} from '@angular/core';
import {StorageService} from "../services/storage.service";
import {TOKEN_KEY} from "../constants/keys.constants";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

    private readonly storageService = inject(StorageService);

    intercept(req: HttpRequest<any>, next: HttpHandler) {
        const token = this.storageService.getStorageItem(TOKEN_KEY);
        if (token) {
            req = req.clone({
                setHeaders: {Authorization: `Bearer ${token}`}
            });
        }
        return next.handle(req);
    }
}
