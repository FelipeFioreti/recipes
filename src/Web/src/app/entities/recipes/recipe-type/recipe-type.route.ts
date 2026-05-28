import {Routes} from '@angular/router';

export const RECIPE_TYPES_ROUTES: Routes = [
    {
        path: '',
        loadComponent: () =>
            import('./recipe-types-list-page.component').then(
                (m) => m.RecipeTypesListPageComponent
            )
    },
    {
        path: 'new',
        loadComponent: () =>
            import('./recipe-type-create-page.component').then(
                (m) => m.RecipeTypeCreatePageComponent
            )
    }
];