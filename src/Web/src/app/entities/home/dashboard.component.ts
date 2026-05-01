import {DatePipe} from '@angular/common';
import {ChangeDetectionStrategy, Component, computed, OnInit, signal} from '@angular/core';
import {Router, RouterLink} from '@angular/router';
import {catchError, finalize, forkJoin, of} from 'rxjs';
import {PageHeaderComponent} from "../../shared/components/page-header/page-header.component";
import {EmptyStateComponent} from "../../shared/components/empty-state/empty-state.component";
import {StatCardComponent} from "../../shared/components/stat-card/stat-card.component";
import {Recipe} from "../../core/models/recipe.model";
import {RecipeType} from "../../core/models/recipe-type.model";
import {RecipeTypesService} from "../../core/services/recipe-types.service";
import {RecipesService} from "../../core/services/recipes.service";
import {AuthStore} from "../../core/services/auth.store";

@Component({
    selector: 'app-dashboard',
    standalone: true,
    imports: [RouterLink, DatePipe, PageHeaderComponent, StatCardComponent, EmptyStateComponent],
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardComponent implements OnInit {

    readonly loading = signal(true);
    readonly recipes = signal<Recipe[]>([]);
    readonly recipeTypes = signal<RecipeType[]>([]);
    readonly activeRecipes = computed(() => this.recipes().filter((recipe) => !recipe.deletedAt));
    readonly archivedRecipes = computed(() => this.recipes().filter((recipe) => !!recipe.deletedAt));
    readonly recentRecipes = computed(() =>
        [...this.activeRecipes()]
            .sort((left, right) => Date.parse(right.updatedAt) - Date.parse(left.updatedAt))
            .slice(0, 4)
    );
    readonly categoryCoverage = computed(() => {
        if (this.authStore.isAdmin()) {
            return this.recipeTypes().filter((type) => !type.deletedAt).length;
        }

        return new Set(this.activeRecipes().map((recipe) => recipe.recipeTypeId)).size;
    });

    public constructor(
        private recipeTypesService: RecipeTypesService,
        private recipeService: RecipesService,
        public authStore: AuthStore,
        private router: Router,
    ) {

    }

    ngOnInit() {
        this.loadData();
    }

    goToRecipes(): void {
        void this.router.navigateByUrl('/app/recipes');
    }

    private loadData(): void {
        forkJoin({
            recipes: this.recipeService.getAll(),
            recipeTypes: this.authStore.isAdmin()
                ? this.recipeTypesService.getAll().pipe(catchError(() => of([])))
                : of([])
        })
            .pipe(finalize(() => this.loading.set(false)))
            .subscribe({
                next: ({recipes, recipeTypes}) => {
                    this.recipes.set(recipes);
                    this.recipeTypes.set(recipeTypes);
                }
            });
    }
}
