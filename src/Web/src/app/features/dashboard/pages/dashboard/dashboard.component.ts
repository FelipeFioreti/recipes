import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { catchError, finalize, forkJoin, of } from 'rxjs';
import { AuthStore } from '../../../../core/services/auth.store';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header/page-header.component';
import { StatCardComponent } from '../../../../shared/components/stat-card/stat-card.component';
import { RecipeType } from '../../../recipe-types/models/recipe-type.model';
import { RecipeTypesService } from '../../../recipe-types/services/recipe-types.service';
import { Recipe } from '../../../recipes/models/recipe.model';
import { RecipesService } from '../../../recipes/services/recipes.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink, DatePipe, PageHeaderComponent, StatCardComponent, EmptyStateComponent],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardComponent {
  private readonly recipesService = inject(RecipesService);
  private readonly recipeTypesService = inject(RecipeTypesService);
  private readonly router = inject(Router);
  readonly authStore = inject(AuthStore);

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

  constructor() {
    this.loadData();
  }

  goToRecipes(): void {
    void this.router.navigateByUrl('/app/recipes');
  }

  private loadData(): void {
    forkJoin({
      recipes: this.recipesService.getAll(),
      recipeTypes: this.authStore.isAdmin()
        ? this.recipeTypesService.getAll().pipe(catchError(() => of([])))
        : of([])
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: ({ recipes, recipeTypes }) => {
          this.recipes.set(recipes);
          this.recipeTypes.set(recipeTypes);
        }
      });
  }
}
