import {Component, DestroyRef, inject, OnInit, signal} from '@angular/core';
import {HttpHeaders, HttpResponse} from '@angular/common/http';
import {ActivatedRoute, Router, RouterModule} from '@angular/router';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {CommonModule} from '@angular/common';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';
import {TranslateModule} from '@ngx-translate/core';
import {NgbPaginationModule} from '@ng-bootstrap/ng-bootstrap';

import {ITEMS_PER_PAGE} from '../../../core/constants/pagination.constants';
import {IRecipeType} from '../../../core/models/recipe-type.model';
import {RecipeTypeActionsService} from './recipe-type-actions.service';
import {RecipeTypesService} from '../../../core/services/recipe-types.service';
import {PageHeaderComponent} from '../../../shared/components/page-header/page-header.component';

@Component({
    selector: 'app-rec-recipe',
    templateUrl: './recipe-type.component.html',
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
export class RecipeTypeComponent implements OnInit {
    recipeTypes = signal<IRecipeType[]>([]);
    totalItems = signal(0);
    itemsPerPage = ITEMS_PER_PAGE;
    page = signal(1);

    private readonly destroyRef = inject(DestroyRef);
    private readonly activatedRoute = inject(ActivatedRoute);
    private readonly router = inject(Router);
    private readonly recipeTypeService = inject(RecipeTypesService);
    public readonly recipeActionsService = inject(RecipeTypeActionsService);

    ngOnInit(): void {
        this.activatedRoute.queryParamMap
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe((params) => {
                const page = Number(params.get('page') ?? 1);
                this.page.set(page > 0 ? page : 1);
                this.loadAll();
            });
    }

    goToView(recipeType: IRecipeType): void {
        this.recipeActionsService.goToView(recipeType);
    }

    goToEdit(recipeType: IRecipeType): void {
        this.recipeActionsService.goToEdit(recipeType);
    }

    delete(recipeType: IRecipeType): void {
        this.recipeActionsService.delete(recipeType, () => this.loadAll());
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
        this.recipeTypeService
            .getAllPaged({
                page: this.page() - 1,
                size: this.itemsPerPage,
            })
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe((response: HttpResponse<IRecipeType[]>) => {
                this.onSuccess(response.body, response.headers);
            });
    }

    private onSuccess(recipeTypes: IRecipeType[] | null, headers: HttpHeaders): void {
        this.totalItems.set(Number(headers.get('X-Total-Count')) || recipeTypes?.length || 0);
        this.recipeTypes.set(recipeTypes ?? []);
    }
}
