import {Routes} from '@angular/router';
import {SidebarComponent} from "../../layout/sidebar/sidebar-component";
import {AuthGuard} from "../../core/guards/auth.guard";

export const HOME_ROUTES: Routes = [
    {
        path: '',
        component: SidebarComponent,
        canActivate: [AuthGuard],
        children: [
            {
                path: 'home',
                loadComponent: () =>
                    import('./home.component').then(
                        (m) => m.HomeComponent
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