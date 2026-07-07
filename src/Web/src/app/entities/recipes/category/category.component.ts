import {Component, computed, DestroyRef, HostListener, inject, OnInit, signal} from '@angular/core';
import {HttpHeaders, HttpResponse} from '@angular/common/http';
import {ActivatedRoute, Router, RouterModule} from '@angular/router';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {CommonModule} from '@angular/common';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';
import {TranslateModule} from '@ngx-translate/core';
import {NgbPaginationModule} from '@ng-bootstrap/ng-bootstrap';

import {ITEMS_PER_PAGE} from '../../../core/constants/pagination.constants';
import {ICategory} from '../../../core/models/category.model';
import {CategoryActionsService} from './category-actions.service';
import {CategoriesService} from '../../../core/services/categories.service';
import {PageHeaderComponent} from '../../../shared/components/page-header/page-header.component';

@Component({
    selector: 'app-rec-category',
    templateUrl: './category.component.html',
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
export class CategoryComponent implements OnInit {
    categories = signal<ICategory[]>([]);
    totalItems = signal(0);
    itemsPerPage = ITEMS_PER_PAGE;
    page = signal(1);
    isMobile = signal(false);
    paginationMaxSize = computed(() => (this.isMobile() ? 3 : 5));

    private readonly destroyRef = inject(DestroyRef);
    private readonly activatedRoute = inject(ActivatedRoute);
    private readonly router = inject(Router);
    private readonly categoriesService = inject(CategoriesService);
    public readonly categoryActionsService = inject(CategoryActionsService);

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

    goToView(category: ICategory): void {
        this.categoryActionsService.goToView(category);
    }

    goToEdit(category: ICategory): void {
        this.categoryActionsService.goToEdit(category);
    }

    delete(category: ICategory): void {
        this.categoryActionsService.delete(category, () => this.loadAll());
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
        this.categoriesService
            .getAllPaged({
                page: this.page() - 1,
                size: this.itemsPerPage,
            })
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe((response: HttpResponse<ICategory[]>) => {
                this.onSuccess(response.body, response.headers);
            });
    }

    private onSuccess(categories: ICategory[] | null, headers: HttpHeaders): void {
        this.totalItems.set(Number(headers.get('X-Total-Count')) || categories?.length || 0);
        this.categories.set(categories ?? []);
    }

    private updateViewportState(): void {
        this.isMobile.set(typeof window !== 'undefined' && window.innerWidth <= 767);
    }
}
