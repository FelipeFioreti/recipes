import {HttpErrorResponse} from '@angular/common/http';
import {ChangeDetectionStrategy, Component, signal} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {Router, RouterLink} from '@angular/router';
import {finalize} from 'rxjs';
import {SaveRecipePayload} from '../../core/models/recipe.model';
import {RecipeType} from '../../core/models/recipe-type.model';
import {NotificationService} from '../../core/services/notification.service';
import {RecipeTypesService} from '../../core/services/recipe-types.service';
import {RecipesService} from '../../core/services/recipes.service';
import {PageHeaderComponent} from '../../shared/components/page-header/page-header.component';

@Component({
    selector: 'app-recipe-create-page',
    standalone: true,
    imports: [ReactiveFormsModule, RouterLink, PageHeaderComponent],
    templateUrl: './recipe-create-page.component.html',
    styleUrls: ['./recipe-create-page.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class RecipeCreatePageComponent {
    readonly recipeTypes = signal<RecipeType[]>([]);
    readonly loadingTypes = signal(true);
    readonly saving = signal(false);
    readonly categoriesUnavailableMessage = signal('');

    readonly form = this.formBuilder.nonNullable.group({
        name: ['', [Validators.required, Validators.maxLength(255)]],
        description: ['', [Validators.maxLength(2000)]],
        recipeTypeId: [0, [Validators.required, Validators.min(1)]]
    });

    public constructor(
        private formBuilder: FormBuilder,
        private recipeTypesService: RecipeTypesService,
        private recipesService: RecipesService,
        private notificationService: NotificationService,
        private router: Router
    ) {
        this.loadRecipeTypes();
    }

    saveRecipe(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.saving.set(true);
        this.recipesService.create(this.form.getRawValue() as SaveRecipePayload)
            .pipe(finalize(() => this.saving.set(false)))
            .subscribe({
                next: () => {
                    this.notificationService.success('Receita criada', 'A receita foi cadastrada com sucesso.');
                    void this.router.navigateByUrl('/app/recipes');
                }
            });
    }

    private loadRecipeTypes(): void {
        this.loadingTypes.set(true);
        this.recipeTypesService.getAll().pipe(finalize(() => this.loadingTypes.set(false))).subscribe({
            next: (recipeTypes) => {
                const activeRecipeTypes = recipeTypes.filter((type) => !type.deletedAt);
                this.recipeTypes.set(activeRecipeTypes);
                if (activeRecipeTypes.length > 0) {
                    this.form.patchValue({recipeTypeId: activeRecipeTypes[0].id});
                }
            },
            error: (error: HttpErrorResponse) => {
                this.recipeTypes.set([]);
                this.form.patchValue({recipeTypeId: 0});
                this.categoriesUnavailableMessage.set(
                    error.status === 403
                        ? 'Sem permissao para carregar tipos de receita.'
                        : 'Nao foi possivel carregar os tipos de receita.'
                );
            }
        });
    }
}
