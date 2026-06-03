import {Routes} from "@angular/router";

export const RECIPES_ROUTES: Routes = [
    {
        path: 'recipes',
        loadComponent: () =>
            import('./recipe/recipe.module').then(
                (m) => m.AppRecipeModule
            )
    },
    {
        path: 'recipes-types',
        loadComponent: () =>
            import('./recipe-type/recipe-type.module').then(
                (m) => m.AppRecipeTypeModule
            )
    }
];