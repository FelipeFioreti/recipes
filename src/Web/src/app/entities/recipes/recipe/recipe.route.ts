import {ResolveFn, Router, Routes} from '@angular/router';
import {IRecipe, Recipe} from "../../../core/models/recipe.model";
import {RecipeService} from "../../../core/services/recipeService";
import {catchError, EMPTY, mergeMap, of} from "rxjs";
import {HttpResponse} from "@angular/common/http";
import {RecipeComponent} from "./recipe.component";
import {AuthGuard} from "../../../core/guards/auth.guard";
import {RecipeUpdateComponent} from "./recipe-update.component";
import {RecipeDetailComponent} from "./recipe-detail.component";
import {pagingParamsResolver} from "../../../core/resolve/paging-params.resolver";
import {inject} from "@angular/core";

export const recipeResolve: ResolveFn<IRecipe> = (route) => {

    const service = inject(RecipeService);
    const router = inject(Router);

    const id = route.params['id'];
    if (id) {
        return service.get(id).pipe(
            mergeMap((recipe: HttpResponse<Recipe>) => {
                if (recipe.body) {
                    return of(recipe.body);
                } else {
                    router.navigate(['404']);
                    return EMPTY;
                }
            }),
            catchError(() => {

                router.navigate(['404']);

                return EMPTY;
            })
        );
    }
    return of(new Recipe());
}


export const recipeRoute: Routes = [
    {
        path: '',
        component: RecipeComponent,
        resolve: {
            pagingParams: pagingParamsResolver
        },
        data: {
            pageTitle: 'recipe.home.title',
        },
        canActivate: [AuthGuard]
    },
    {
        path: ':id/view',
        component: RecipeDetailComponent,
        resolve: {
            recipe: recipeResolve
        },
        data: {
            pageTitle: 'recipe.home.title',
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'new',
        component: RecipeUpdateComponent,
        resolve: {
            recipe: recipeResolve
        },
        data: {
            pageTitle: 'recipe.home.title',
        },
        canActivate: [AuthGuard]
    },
    {
        path: ':id/edit',
        component: RecipeUpdateComponent,
        resolve: {
            recipe: recipeResolve
        },
        data: {
            pageTitle: 'recipe.home.title',
        },
        canActivate: [AuthGuard]
    }
];
