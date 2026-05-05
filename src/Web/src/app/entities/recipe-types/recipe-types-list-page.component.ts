import {DatePipe} from '@angular/common';
import {ChangeDetectionStrategy, Component, computed, signal} from '@angular/core';
import {RouterLink} from '@angular/router';
import {finalize} from 'rxjs';
import {RecipeType} from '../../core/models/recipe-type.model';
import {RecipeTypesService} from '../../core/services/recipe-types.service';
import {PageHeaderComponent} from '../../shared/components/page-header/page-header.component';

@Component({
    selector: 'app-recipe-types-list-page',
    standalone: true,
    imports: [DatePipe, RouterLink, PageHeaderComponent],
    templateUrl: './recipe-types-list-page.component.html',
    styleUrls: ['./recipe-types-list-page.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class RecipeTypesListPageComponent {
    readonly pageSize = 10;
    readonly loading = signal(true);
    readonly currentPage = signal(1);
    readonly recipeTypes = signal<RecipeType[]>([]);

    readonly sortedRecipeTypes = computed(() =>
        [...this.recipeTypes()].sort((left, right) => Date.parse(right.updatedAt) - Date.parse(left.updatedAt))
    );
    readonly totalPages = computed(() => Math.max(1, Math.ceil(this.sortedRecipeTypes().length / this.pageSize)));
    readonly paginatedRecipeTypes = computed(() => {
        const start = (this.currentPage() - 1) * this.pageSize;
        return this.sortedRecipeTypes().slice(start, start + this.pageSize);
    });

    public constructor(private recipeTypesService: RecipeTypesService) {
        this.loadData();
    }

    nextPage(): void {
        this.currentPage.update((page) => Math.min(page + 1, this.totalPages()));
    }

    previousPage(): void {
        this.currentPage.update((page) => Math.max(page - 1, 1));
    }

    private loadData(): void {
        this.loading.set(true);
        this.recipeTypesService.getAll().pipe(finalize(() => this.loading.set(false))).subscribe({
            next: (recipeTypes) => this.recipeTypes.set(recipeTypes)
        });
    }
}
