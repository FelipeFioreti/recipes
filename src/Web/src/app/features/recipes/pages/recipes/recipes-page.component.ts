import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { NotificationService } from '../../../../core/services/notification.service';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header/page-header.component';
import { StatCardComponent } from '../../../../shared/components/stat-card/stat-card.component';
import { RecipeType } from '../../../recipe-types/models/recipe-type.model';
import { RecipeTypesService } from '../../../recipe-types/services/recipe-types.service';
import { RecipeEditorComponent } from '../../components/recipe-editor/recipe-editor.component';
import { RecipeCardComponent } from '../../components/recipe-card/recipe-card.component';
import { Recipe, SaveRecipePayload } from '../../models/recipe.model';
import { RecipesService } from '../../services/recipes.service';

@Component({
  selector: 'app-recipes-page',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    PageHeaderComponent,
    StatCardComponent,
    EmptyStateComponent,
    RecipeCardComponent,
    RecipeEditorComponent
  ],
  templateUrl: './recipes-page.component.html',
  styleUrls: ['./recipes-page.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RecipesPageComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly recipesService = inject(RecipesService);
  private readonly recipeTypesService = inject(RecipeTypesService);
  private readonly notificationService = inject(NotificationService);

  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly mode = signal<'create' | 'edit'>('create');
  readonly selectedRecipeId = signal<number | null>(null);
  readonly searchTerm = signal('');
  readonly recipes = signal<Recipe[]>([]);
  readonly recipeTypes = signal<RecipeType[]>([]);
  readonly categoriesUnavailableMessage = signal('');

  readonly form = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(255)]],
    description: ['', [Validators.maxLength(2000)]],
    recipeTypeId: [0, [Validators.required, Validators.min(1)]]
  });

  readonly sortedRecipes = computed(() =>
    [...this.recipes()].sort((left, right) => Date.parse(right.updatedAt) - Date.parse(left.updatedAt))
  );

  readonly filteredRecipes = computed(() => {
    const query = this.searchTerm().trim().toLowerCase();

    return this.sortedRecipes().filter((recipe) => {
      if (!query) {
        return true;
      }

      return `${recipe.name} ${recipe.description}`.toLowerCase().includes(query);
    });
  });

  readonly categoryMap = computed(() => new Map(this.recipeTypes().map((type) => [type.id, type.name])));
  readonly activeRecipesCount = computed(() => this.recipes().filter((recipe) => !recipe.deletedAt).length);
  readonly archivedRecipesCount = computed(() => this.recipes().filter((recipe) => !!recipe.deletedAt).length);
  readonly recentRecipesCount = computed(() => {
    const threshold = Date.now() - 1000 * 60 * 60 * 24 * 7;

    return this.recipes().filter((recipe) => Date.parse(recipe.updatedAt) >= threshold).length;
  });

  constructor() {
    this.loadRecipes();
    this.loadRecipeTypes();
  }

  updateSearchTerm(term: string): void {
    this.searchTerm.set(term);
  }

  startCreate(): void {
    this.mode.set('create');
    this.selectedRecipeId.set(null);
    this.form.reset({
      name: '',
      description: '',
      recipeTypeId: this.recipeTypes()[0]?.id ?? 0
    });
  }

  startEdit(recipe: Recipe): void {
    this.mode.set('edit');
    this.selectedRecipeId.set(recipe.id);
    this.form.setValue({
      name: recipe.name,
      description: recipe.description ?? '',
      recipeTypeId: recipe.recipeTypeId
    });
  }

  saveRecipe(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = this.form.getRawValue() as SaveRecipePayload;
    const recipeId = this.selectedRecipeId();
    const request$ =
      this.mode() === 'edit' && recipeId
        ? this.recipesService.update(recipeId, payload)
        : this.recipesService.create(payload);

    this.saving.set(true);

    request$
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: (recipe) => {
          if (this.mode() === 'edit') {
            this.recipes.update((recipes) => recipes.map((item) => (item.id === recipe.id ? recipe : item)));
            this.notificationService.success('Receita atualizada', 'As alteracoes foram salvas com sucesso.');
          } else {
            this.recipes.update((recipes) => [recipe, ...recipes]);
            this.notificationService.success('Receita criada', 'A nova receita ja esta disponivel no catalogo.');
          }

          this.startCreate();
        }
      });
  }

  archiveRecipe(recipe: Recipe): void {
    if (recipe.deletedAt) {
      return;
    }

    this.recipesService.delete(recipe.id).subscribe({
      next: () => {
        this.recipes.update((recipes) =>
          recipes.map((item) =>
            item.id === recipe.id
              ? {
                  ...item,
                  deletedAt: new Date().toISOString(),
                  updatedAt: new Date().toISOString()
                }
              : item
          )
        );

        if (this.selectedRecipeId() === recipe.id) {
          this.startCreate();
        }

        this.notificationService.success('Receita arquivada', 'A receita continua visivel no historico, mas foi marcada como arquivada.');
      }
    });
  }

  categoryName(recipeTypeId: number): string {
    return this.categoryMap().get(recipeTypeId) ?? 'Categoria nao carregada';
  }

  private loadRecipes(): void {
    this.loading.set(true);

    this.recipesService
      .getAll()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (recipes) => {
          this.recipes.set(recipes);
        }
      });
  }

  private loadRecipeTypes(): void {
    this.recipeTypesService.getAll().subscribe({
      next: (recipeTypes) => {
        const activeRecipeTypes = recipeTypes.filter((type) => !type.deletedAt);
        this.recipeTypes.set(activeRecipeTypes);

        if (activeRecipeTypes.length > 0) {
          this.form.patchValue({
            recipeTypeId: activeRecipeTypes[0].id
          });
        }
      },
      error: (error: HttpErrorResponse) => {
        this.recipeTypes.set([]);
        this.form.patchValue({ recipeTypeId: 0 });
        this.categoriesUnavailableMessage.set(
          error.status === 403
            ? 'A API atual restringe o carregamento de categorias a administradores. Sem categorias disponiveis, a criacao de novas receitas fica bloqueada.'
            : 'Nao foi possivel carregar as categorias neste momento.'
        );
      }
    });
  }
}

