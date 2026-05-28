import {Routes} from "@angular/router";

export const RECIPES_ROUTES: Routes = [
    {
        path: '',
        loadComponent: () =>
            import('./recipe/recipe.module').then(
                (m) => m.AppRecipeModule
            )
    },
    {
        path: 'new',
        loadComponent: () =>
            import('./recipe-create-page.component').then(
                (m) => m.RecipeCreatePageComponent
            )
    }
];