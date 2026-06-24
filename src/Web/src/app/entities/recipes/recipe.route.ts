import {Routes} from "@angular/router";

export const RECIPES_ROUTES: Routes = [
    {
        path: 'recipes',
        loadChildren: () =>
            import('./recipe/recipe.route').then((m) => m.recipeRoute)
    },
    {
        path: 'recipes-types',
        loadChildren: () =>
            import('./recipe-type/recipe-type.route').then((m) => m.recipeTypeRoute)
    }
];
