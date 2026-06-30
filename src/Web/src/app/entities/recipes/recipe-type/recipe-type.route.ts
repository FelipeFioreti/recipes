import {ResolveFn, Router, Routes} from '@angular/router';
import {catchError, EMPTY, mergeMap, of} from "rxjs";
import {RecipeTypeComponent} from "./recipe-type.component";
import {AuthGuard} from "../../../core/guards/auth.guard";
import {RecipeTypeUpdateComponent} from "./recipe-type-update.component";
import {RecipeTypeDetailComponent} from "./recipe-type-detail.component";
import {inject} from "@angular/core";
import {IRecipeType, RecipeType} from "../../../core/models/recipe-type.model";
import {RecipeTypesService} from "../../../core/services/recipe-types.service";

export const recipeTypeResolve: ResolveFn<IRecipeType> = (route) => {

    const service = inject(RecipeTypesService);
    const router = inject(Router);

    const id = route.params['id'];
    if (id) {
        return service.get(id).pipe(
            mergeMap((recipeType: RecipeType) => {
                if (recipeType) {
                    return of(recipeType);
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
    return of(new RecipeType());
}


export const recipeTypeRoute: Routes = [
    {
        path: '',
        component: RecipeTypeComponent,
        data: {
            pageTitle: 'recipeType.home.title',
        },
        canActivate: [AuthGuard]
    },
    {
        path: ':id/view',
        component: RecipeTypeDetailComponent,
        resolve: {
            recipeType: recipeTypeResolve
        },
        data: {
            pageTitle: 'recipeType.home.title',
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'new',
        component: RecipeTypeUpdateComponent,
        resolve: {
            recipeType: recipeTypeResolve
        },
        data: {
            pageTitle: 'recipeType.home.title',
        },
        canActivate: [AuthGuard]
    },
    {
        path: ':id/edit',
        component: RecipeTypeUpdateComponent,
        resolve: {
            recipeType: recipeTypeResolve
        },
        data: {
            pageTitle: 'recipeType.home.title',
        },
        canActivate: [AuthGuard]
    }
];
