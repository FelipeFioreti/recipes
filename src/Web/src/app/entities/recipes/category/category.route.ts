import {ResolveFn, Router, Routes} from '@angular/router';
import {catchError, EMPTY, mergeMap, of} from "rxjs";
import {CategoryComponent} from "./category.component";
import {AuthGuard} from "../../../core/guards/auth.guard";
import {CategoryUpdateComponent} from "./category-update.component";
import {CategoryDetailComponent} from "./category-detail.component";
import {inject} from "@angular/core";
import {Category, ICategory} from "../../../core/models/category.model";
import {CategoriesService} from "../../../core/services/categories.service";
import {pagingParamsResolver} from "../../../core/resolve/paging-params.resolver";

export const categoryResolve: ResolveFn<ICategory> = (route) => {
    const service = inject(CategoriesService);
    const router = inject(Router);

    const id = route.params['id'];
    if (id) {
        return service.get(id).pipe(
            mergeMap((category: Category) => {
                if (category) {
                    return of(category);
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
    return of(new Category());
}

export const categoryRoute: Routes = [
    {
        path: '',
        component: CategoryComponent,
        resolve: {
            pagingParams: pagingParamsResolver
        },
        data: {
            pageTitle: 'category.home.title',
        },
        canActivate: [AuthGuard]
    },
    {
        path: ':id/view',
        component: CategoryDetailComponent,
        resolve: {
            category: categoryResolve
        },
        data: {
            pageTitle: 'category.home.title',
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'new',
        component: CategoryUpdateComponent,
        resolve: {
            category: categoryResolve
        },
        data: {
            pageTitle: 'category.home.title',
        },
        canActivate: [AuthGuard]
    },
    {
        path: ':id/edit',
        component: CategoryUpdateComponent,
        resolve: {
            category: categoryResolve
        },
        data: {
            pageTitle: 'category.home.title',
        },
        canActivate: [AuthGuard]
    }
];
