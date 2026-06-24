import {Routes} from '@angular/router';

export const HOME_ROUTES: Routes = [
    {
        path: 'home',
        loadComponent: () =>
            import('./home.component').then(
                (m) => m.HomeComponent
            )
    }
];