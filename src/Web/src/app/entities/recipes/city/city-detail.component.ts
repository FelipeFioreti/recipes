import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';

import {ICity} from 'app/shared/model/location/city.model';
import {Subscription} from 'rxjs';
import {PageService} from 'app/shared/layout/window/page.service';
import {Page} from 'app/shared/layout/window/page.model';
import {ButtonBuilder} from 'app/shared/layout/window/button-builder';
import {unsubscribe} from 'app/shared/util/react-util';
import {CityActionsService} from 'app/entities/location/city/city-actions.service';

@Component({
    selector: 'app-loc-city-detail',
    templateUrl: './recipe-detail.component.html'
})
export class CityDetailComponent implements OnInit, OnDestroy {
    city: ICity | null = null;

    private subscriptions: Subscription[] = [];

    constructor(private route: ActivatedRoute, private pageService: PageService, private cityActionsService: CityActionsService) {
    }

    ngOnInit(): void {
        this.subscriptions.push(this.route.data.subscribe(({city}) => (this.city = city)));
        this.setupPage();
    }

    private setupPage(): void {
        this.pageService.setCurrentPage(
            new Page(
                () => 'map-signs',
                () => 'sidebar.admin.city',
                () => (this.city ? this.city.name! : ''),
                () => undefined,
                () => false,
                [
                    ButtonBuilder.back(() => this.cityActionsService.goToList()),
                    ButtonBuilder.edit(
                        () => this.city && this.cityActionsService.goToEdit(this.city),
                        () => !!this.city && this.cityActionsService.hasPrivilegeToEdit()
                    ),
                    ButtonBuilder.new(
                        () => this.cityActionsService.goToNew(),
                        () => this.cityActionsService.hasPrivilegeToCreate()
                    )
                ]
            )
        );
    }

    ngOnDestroy(): void {
        unsubscribe(this.subscriptions);
    }
}
