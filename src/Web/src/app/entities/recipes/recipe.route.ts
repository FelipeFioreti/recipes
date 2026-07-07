import {Routes} from "@angular/router";

export const RECIPES_ROUTES: Routes = [
    {
        path: 'recipes',
        loadChildren: () =>
            import('./recipe/recipe.route').then((m) => m.recipeRoute)
    },
    {
        path: 'categories',
        loadChildren: () =>
            import('./category/category.route').then((m) => m.categoryRoute)
    }
];
