import {ApplicationConfig, provideZoneChangeDetection} from '@angular/core';
import {HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi} from '@angular/common/http';
import {provideRouter} from '@angular/router';
import {routes} from './app.routes';
import {AuthInterceptor} from "./core/interceptors/auth.interceptor";
import {provideTranslateService} from "@ngx-translate/core";
import {provideTranslateHttpLoader} from "@ngx-translate/http-loader";
import {fontAwesomeProviders} from "./config/font-awesome.config";

export const appConfig: ApplicationConfig = {
    providers: [
        provideHttpClient(withInterceptorsFromDi()),
        provideZoneChangeDetection({eventCoalescing: true}),
        provideTranslateService({
            loader: provideTranslateHttpLoader({
                prefix: './assets/i18n/',
                suffix: '.json'
            }),
            lang: 'pt'
        }),
        provideRouter(routes),
        {provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true},
        ...fontAwesomeProviders
    ]
};

