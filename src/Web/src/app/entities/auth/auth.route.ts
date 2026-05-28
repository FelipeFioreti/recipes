import {Routes} from '@angular/router';
import {AuthLayoutComponent} from '../../layout/auth-layout/auth-layout.component';

export const AUTH_ROUTES: Routes = [
    {
        path: '',
        component: AuthLayoutComponent,
        children: [
            {
                path: 'login',
                loadComponent: () =>
                    import('./login/login.component').then(
                        (m) => m.LoginComponent
                    )
            },
            {
                path: 'sign-up',
                loadComponent: () =>
                    import('./sign-up/sign-up.component').then(
                        (m) => m.SignUpComponent
                    )
            },
            {
                path: '',
                pathMatch: 'full',
                redirectTo: 'login'
            }
        ]
    }
];