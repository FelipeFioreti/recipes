import {ResolveFn} from '@angular/router';

export interface IPagingParams {
    page: number;
    predicate: string;
    ascending: boolean;
}

export const pagingParamsResolver: ResolveFn<IPagingParams> = (route) => {

    const page = Number(route.queryParamMap.get('page') ?? 1);

    const sort = route.queryParamMap.get('sort') ?? 'id,asc';

    const [predicate, direction] = sort.split(',');

    return {
        page,
        predicate,
        ascending: direction === 'asc'
    };
};