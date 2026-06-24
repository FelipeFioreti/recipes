import {Routes} from '@angular/router';

export const AUTH_ROUTES: Routes = [
    {
        path: 'login',
        loadChildren: () =>
            import('./login/login.route').then((m) => m.loginRoute)
    },
    {
        path: 'sign-up',
        loadChildren: () =>
            import('./sign-up/sign-up.route').then((m) => m.signUpRoute)
    }
]