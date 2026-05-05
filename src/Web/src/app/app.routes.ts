import {Routes} from '@angular/router';
import {authGuard} from './core/guards/auth.guard';
import {guestGuard} from './core/guards/guest.guard';
import {adminGuard} from './core/guards/admin.guard';
import {AuthLayoutComponent} from './layout/auth-layout/auth-layout.component';
import {CommonLayoutComponent} from "./layout/common-layout/common-layout.component";

export const routes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        redirectTo: 'app/home'
    },
    {
        path: 'auth',
        component: AuthLayoutComponent,
        canActivate: [guestGuard],
        children: [
            {
                path: 'sign-in',
                loadComponent: () =>
                    import('./entities/auth/sign-in/sign-in.component').then((m) => m.SignInComponent)
            },
            {
                path: 'sign-up',
                loadComponent: () =>
                    import('./entities/auth/sign-up/sign-up.component').then((m) => m.SignUpComponent)
            },
            {
                path: '',
                pathMatch: 'full',
                redirectTo: 'sign-in'
            }
        ]
    },
    {
        path: 'app',
        component: CommonLayoutComponent,
        canActivate: [authGuard],
        children: [
            {
                path: 'home',
                loadComponent: () =>
                    import('./entities/home/home.component').then((m) => m.HomeComponent)
            },
            {
                path: 'recipes',
                loadComponent: () =>
                    import('./entities/recipes/recipes-list-page.component').then((m) => m.RecipesListPageComponent)
            },
            {
                path: 'recipes/new',
                loadComponent: () =>
                    import('./entities/recipes/recipe-create-page.component').then((m) => m.RecipeCreatePageComponent)
            },
            {
                path: 'recipe-types',
                canActivate: [adminGuard],
                loadComponent: () =>
                    import('./entities/recipe-types/recipe-types-list-page.component').then(
                        (m) => m.RecipeTypesListPageComponent
                    )
            },
            {
                path: 'recipe-types/new',
                canActivate: [adminGuard],
                loadComponent: () =>
                    import('./entities/recipe-types/recipe-type-create-page.component').then(
                        (m) => m.RecipeTypeCreatePageComponent
                    )
            },
            {
                path: '',
                pathMatch: 'full',
                redirectTo: 'home'
            }
        ]
    },
    {
        path: '**',
        redirectTo: 'app/home'
    }
];

