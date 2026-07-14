import {Component, computed, DestroyRef, HostListener, inject, OnInit, signal} from '@angular/core';
import {HttpHeaders, HttpResponse} from '@angular/common/http';
import {ActivatedRoute, Router, RouterModule} from '@angular/router';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {CommonModule} from '@angular/common';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';
import {TranslateModule} from '@ngx-translate/core';
import {NgbPaginationModule} from '@ng-bootstrap/ng-bootstrap';

import {ITEMS_PER_PAGE} from '../../../core/constants/pagination.constants';
import {IUnit} from '../../../core/models/unit.model';
import {UnitsService} from '../../../core/services/units.service';
import {PageHeaderComponent} from '../../../shared/components/page-header/page-header.component';
import {UnitActionsService} from './unit-actions.service';

@Component({
    selector: 'app-rec-unit',
    templateUrl: './unit.component.html',
    standalone: true,
    imports: [
        CommonModule,
        RouterModule,
        FontAwesomeModule,
        TranslateModule,
        NgbPaginationModule,
        PageHeaderComponent,
    ],
})
export class UnitComponent implements OnInit {
    units = signal<IUnit[]>([]);
    totalItems = signal(0);
    itemsPerPage = ITEMS_PER_PAGE;
    page = signal(1);
    isMobile = signal(false);
    paginationMaxSize = computed(() => (this.isMobile() ? 3 : 5));

    private readonly destroyRef = inject(DestroyRef);
    private readonly activatedRoute = inject(ActivatedRoute);
    private readonly router = inject(Router);
    private readonly unitsService = inject(UnitsService);
    public readonly unitActionsService = inject(UnitActionsService);

    ngOnInit(): void {
        this.updateViewportState();

        this.activatedRoute.queryParamMap
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe((params) => {
                const page = Number(params.get('page') ?? 1);
                this.page.set(page > 0 ? page : 1);
                this.loadAll();
            });
    }

    @HostListener('window:resize')
    onWindowResize(): void {
        this.updateViewportState();
    }

    goToView(unit: IUnit): void {
        this.unitActionsService.goToView(unit);
    }

    goToEdit(unit: IUnit): void {
        this.unitActionsService.goToEdit(unit);
    }

    delete(unit: IUnit): void {
        this.unitActionsService.delete(unit, () => this.loadAll());
    }

    navigateToPage(page: number): void {
        const nextPage = Math.max(1, page);

        if (nextPage === this.page()) {
            return;
        }

        this.router.navigate([], {
            relativeTo: this.activatedRoute,
            queryParams: {
                page: nextPage === 1 ? null : nextPage,
            },
            queryParamsHandling: 'merge',
        });
    }

    itemRangeStart(): number {
        return this.totalItems() === 0 ? 0 : (this.page() - 1) * this.itemsPerPage + 1;
    }

    itemRangeEnd(): number {
        return Math.min(this.page() * this.itemsPerPage, this.totalItems());
    }

    private loadAll(): void {
        this.unitsService
            .getAllPaged({
                page: this.page() - 1,
                size: this.itemsPerPage,
            })
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe((response: HttpResponse<IUnit[]>) => {
                this.onSuccess(response.body, response.headers);
            });
    }

    private onSuccess(units: IUnit[] | null, headers: HttpHeaders): void {
        this.totalItems.set(Number(headers.get('X-Total-Count')) || units?.length || 0);
        this.units.set(units ?? []);
    }

    private updateViewportState(): void {
        this.isMobile.set(typeof window !== 'undefined' && window.innerWidth <= 767);
    }
}
