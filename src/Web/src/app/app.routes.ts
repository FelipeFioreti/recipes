import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { adminGuard } from './core/guards/admin.guard';
import { ShellLayoutComponent } from './layout/shell-layout/shell-layout.component';
import { AuthLayoutComponent } from './layout/auth-layout/auth-layout.component';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'app/dashboard'
  },
  {
    path: 'auth',
    component: AuthLayoutComponent,
    canActivate: [guestGuard],
    children: [
      {
        path: 'sign-in',
        loadComponent: () =>
          import('./features/auth/pages/sign-in/sign-in.component').then((m) => m.SignInComponent)
      },
      {
        path: 'sign-up',
        loadComponent: () =>
          import('./features/auth/pages/sign-up/sign-up.component').then((m) => m.SignUpComponent)
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
    component: ShellLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/pages/dashboard/dashboard.component').then((m) => m.DashboardComponent)
      },
      {
        path: 'recipes',
        loadComponent: () =>
          import('./features/recipes/pages/recipes/recipes-page.component').then((m) => m.RecipesPageComponent)
      },
      {
        path: 'categories',
        canActivate: [adminGuard],
        loadComponent: () =>
          import('./features/recipe-types/pages/recipe-types/recipe-types-page.component').then(
            (m) => m.RecipeTypesPageComponent
          )
      },
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'dashboard'
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'app/dashboard'
  }
];

