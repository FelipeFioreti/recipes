import {ResolveFn, Router, Routes} from '@angular/router';
import {catchError, EMPTY, mergeMap, of} from 'rxjs';
import {inject} from '@angular/core';

import {AuthGuard} from '../../../core/guards/auth.guard';
import {pagingParamsResolver} from '../../../core/resolve/paging-params.resolver';
import {IUnit, Unit} from '../../../core/models/unit.model';
import {UnitsService} from '../../../core/services/units.service';
import {UnitComponent} from './unit.component';
import {UnitDetailComponent} from './unit-detail.component';
import {UnitUpdateComponent} from './unit-update.component';

export const unitResolve: ResolveFn<IUnit> = (route) => {
    const service = inject(UnitsService);
    const router = inject(Router);

    const id = route.params['id'];
    if (id) {
        return service.get(id).pipe(
            mergeMap((unit: Unit) => {
                if (unit) {
                    return of(unit);
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
    return of(new Unit());
};

export const unitRoute: Routes = [
    {
        path: '',
        component: UnitComponent,
        resolve: {
            pagingParams: pagingParamsResolver
        },
        data: {
            pageTitle: 'unit.home.title',
        },
        canActivate: [AuthGuard]
    },
    {
        path: ':id/view',
        component: UnitDetailComponent,
        resolve: {
            unit: unitResolve
        },
        data: {
            pageTitle: 'unit.home.title',
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'new',
        component: UnitUpdateComponent,
        resolve: {
            unit: unitResolve
        },
        data: {
            pageTitle: 'unit.home.title',
        },
        canActivate: [AuthGuard]
    },
    {
        path: ':id/edit',
        component: UnitUpdateComponent,
        resolve: {
            unit: unitResolve
        },
        data: {
            pageTitle: 'unit.home.title',
        },
        canActivate: [AuthGuard]
    }
];
