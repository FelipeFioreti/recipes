import {Routes} from '@angular/router';
import {AuthGuard} from './core/guards/auth.guard';
import {SidebarComponent} from './layout/sidebar/sidebar-component';
import {AUTH_ROUTES} from "./entities/auth/auth.route";
import {HOME_ROUTES} from "./entities/home/home.route";
import {RECIPES_ROUTES} from "./entities/recipes/recipe.route";

export const routes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        redirectTo: 'app/home'
    },
    {
        path: 'auth',
        children: AUTH_ROUTES
    },
    {
        path: 'app',
        component: SidebarComponent,
        canActivate: [AuthGuard],
        children: [
            ...HOME_ROUTES,
            ...RECIPES_ROUTES
        ]
    },
    {
        path: '**',
        redirectTo: 'app/home'
    },
];
