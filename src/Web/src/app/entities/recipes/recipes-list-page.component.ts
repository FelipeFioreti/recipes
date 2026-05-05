import {DatePipe} from '@angular/common';
import {ChangeDetectionStrategy, Component, computed, signal} from '@angular/core';
import {RouterLink} from '@angular/router';
import {finalize} from 'rxjs';
import {Recipe} from '../../core/models/recipe.model';
import {RecipeType} from '../../core/models/recipe-type.model';
import {RecipeTypesService} from '../../core/services/recipe-types.service';
import {RecipesService} from '../../core/services/recipes.service';
import {PageHeaderComponent} from '../../shared/components/page-header/page-header.component';

@Component({
    selector: 'app-recipes-list-page',
    standalone: true,
    imports: [DatePipe, RouterLink, PageHeaderComponent],
    templateUrl: './recipes-list-page.component.html',
    styleUrls: ['./recipes-list-page.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class RecipesListPageComponent {
    readonly pageSize = 10;
    readonly loading = signal(true);
    readonly currentPage = signal(1);
    readonly recipes = signal<Recipe[]>([]);
    readonly recipeTypes = signal<RecipeType[]>([]);

    readonly sortedRecipes = computed(() =>
        [...this.recipes()].sort((left, right) => Date.parse(right.updatedAt) - Date.parse(left.updatedAt))
    );
    readonly totalPages = computed(() => Math.max(1, Math.ceil(this.sortedRecipes().length / this.pageSize)));
    readonly paginatedRecipes = computed(() => {
        const start = (this.currentPage() - 1) * this.pageSize;
        return this.sortedRecipes().slice(start, start + this.pageSize);
    });
    readonly categoryMap = computed(() => new Map(this.recipeTypes().map((item) => [item.id, item.name])));

    public constructor(
        private recipesService: RecipesService,
        private recipeTypesService: RecipeTypesService
    ) {
        this.loadData();
    }

    nextPage(): void {
        this.currentPage.update((page) => Math.min(page + 1, this.totalPages()));
    }

    previousPage(): void {
        this.currentPage.update((page) => Math.max(page - 1, 1));
    }

    recipeTypeName(recipeTypeId: number): string {
        return this.categoryMap().get(recipeTypeId) ?? '-';
    }

    private loadData(): void {
        this.loading.set(true);

        this.recipesService.getAll().pipe(finalize(() => this.loading.set(false))).subscribe({
            next: (recipes) => this.recipes.set(recipes)
        });

        this.recipeTypesService.getAll().subscribe({
            next: (types) => this.recipeTypes.set(types)
        });
    }
}
