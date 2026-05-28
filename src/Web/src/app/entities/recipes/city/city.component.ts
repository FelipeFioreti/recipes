import {Component, OnDestroy, OnInit} from '@angular/core';
import {HttpHeaders, HttpResponse} from '@angular/common/http';
import {Subscription} from 'rxjs';

import {City, ICity} from 'app/shared/model/location/city.model';

import {ITEMS_PER_PAGE} from 'app/shared/constants/pagination.constants';
import {CityService} from 'app/shared/services/location/city.service';
import {Account} from 'app/core/user/account.model';
import {AccountService} from 'app/core/auth/account.service';
import {ActivatedRoute, Router} from '@angular/router';
import {PageService} from 'app/shared/layout/window/page.service';
import {map, switchMap} from 'rxjs/operators';
import {Page} from 'app/shared/layout/window/page.model';
import {ButtonBuilder} from 'app/shared/layout/window/button-builder';
import {CityActionsService} from 'app/entities/location/city/city-actions.service';
import {unsubscribe} from 'app/shared/util/react-util';
import {Criteria} from 'app/shared/filter/criteria.model';
import {CriteriaFilterModalService} from 'app/shared/filter/criteria-filter-modal.service';
import {IBaseEntity} from 'app/shared/model/root/base-entity.model';
import {StateService} from 'app/shared/services/location/state.service';
import {IState} from 'app/shared/model/location/state.model';

@Component({
    selector: 'app-loc-city',
    templateUrl: './recipe.component.html'
})
export class CityComponent implements OnInit, OnDestroy {
    currentAccount: Account | null = null;
    cities: ICity[] | null = null;
    totalItems = 0;
    itemsPerPage = ITEMS_PER_PAGE;
    page!: number;
    predicate!: string;
    previousPage!: number;
    ascending!: boolean;
    filter?: Object | null;

    private criterias: Criteria<City>[] = City.criterias();
    private subscriptions: Subscription[] = [];

    constructor(
        private cityService: CityService,
        private accountService: AccountService,
        private activatedRoute: ActivatedRoute,
        private router: Router,
        private pageService: PageService,
        public cityActionsService: CityActionsService,
        private criteriaFilterModalService: CriteriaFilterModalService<City>,
        private stateService: StateService
    ) {
        this.cities = [];
    }

    ngOnInit(): void {
        this.setupPage();
        this.restoreFilter();
        this.subscriptions.push(
            this.accountService
                .identity()
                .pipe(
                    map(account => (this.currentAccount = account)),
                    switchMap(() => this.activatedRoute.data)
                )
                .subscribe(data => {
                    this.page = data.pagingParams.page;
                    this.previousPage = data.pagingParams.page;
                    this.ascending = data.pagingParams.ascending;
                    this.predicate = data.pagingParams.predicate;
                    this.loadAll();
                })
        );
    }

    private setupPage(): void {
        this.pageService.setCurrentPage(
            new Page(
                () => 'map-signs',
                () => 'sidebar.admin.city',
                () => '',
                () => this.loadAll(),
                () => false,
                [
                    ButtonBuilder.filterWithBadge(
                        () =>
                            this.subscriptions.push(
                                this.stateService
                                    .query()
                                    .pipe(
                                        map((res: HttpResponse<IState[]>) => {
                                            const stateCriteria = this.criterias.find(criteria => criteria.property.name === 'state');
                                            stateCriteria && (stateCriteria.items = () => res.body || []);
                                        })
                                    )
                                    .subscribe(() =>
                                        this.criteriaFilterModalService.show(
                                            'city',
                                            this.criterias,
                                            filter => this.updateFilter(filter),
                                            (criterias: Criteria<City>[]) => (this.criterias = criterias)
                                        )
                                    )
                            ),
                        () => !!this.filter
                    ),
                    ButtonBuilder.new(
                        () => this.cityActionsService.goToNew(),
                        () => this.cityActionsService.hasPrivilegeToCreate()
                    )
                ]
            )
        );
    }

    trackIdentity(index: number, item: IBaseEntity): any {
        return item.id ?? item.uuid;
    }

    loadPage(page: number): void {
        if (page !== this.previousPage) {
            this.previousPage = page;
            this.transition();
        }
    }

    transition(): void {
        this.router.navigate(['./'], {
            relativeTo: this.activatedRoute.parent,
            queryParams: {
                page: this.page,
                sort: this.predicate + ',' + (this.ascending ? 'asc' : 'desc')
            }
        });
        this.loadAll();
    }

    goToView(city: ICity): void {
        this.cityActionsService.goToView(city);
    }

    goToEdit(city: ICity): void {
        this.cityActionsService.goToEdit(city);
    }

    delete(city: ICity): void {
        this.cityActionsService.delete(city, () => this.loadAll());
    }

    private updateFilter(filter: Object): void {
        this.filter = Object.keys(filter).length ? filter : null;
        this.saveFilters();
        this.loadAll();
    }

    private restoreFilter(): void {
        this.filter = this.cityActionsService.getSavedFilter();
        this.cityActionsService.restoreCriterias(this.criterias);
    }

    private saveFilters(): void {
        this.cityActionsService.saveCriterias(this.criterias);
        this.cityActionsService.saveFilter(this.filter);
    }

    private loadAll(): void {
        this.subscriptions.push(
            this.cityService
                .query({
                    page: this.page - 1,
                    size: this.itemsPerPage,
                    sort: this.sort(),
                    ...(this.filter || {})
                })
                .subscribe((res: HttpResponse<ICity[]>) => this.onSuccess(res.body, res.headers))
        );
    }

    reset(): void {
        this.page = 0;
        this.cities = [];
        this.loadAll();
    }

    private sort(): string[] {
        const result = [this.predicate + ',' + (this.ascending ? 'asc' : 'desc')];
        if (this.predicate !== 'id') {
            result.push('id');
        }
        return result;
    }

    private onSuccess(cities: ICity[] | null, headers: HttpHeaders): void {
        this.totalItems = Number(headers.get('X-Total-Count'));
        this.cities = cities;
        this.setupPage();
    }

    ngOnDestroy(): void {
        unsubscribe(this.subscriptions);
    }
}
