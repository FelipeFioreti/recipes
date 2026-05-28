import {DatePipe} from '@angular/common';
import {ChangeDetectionStrategy, Component, computed, signal} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {finalize} from 'rxjs';
import {PageHeaderComponent} from "../../../shared/components/page-header/page-header.component";
import {StatCardComponent} from "../../../shared/components/stat-card/stat-card.component";
import {EmptyStateComponent} from "../../../shared/components/empty-state/empty-state.component";
import {NotificationService} from "../../../core/services/notification.service";
import {RecipeTypesService} from "../../../core/services/recipe-types.service";
import {RecipeType, SaveRecipeTypePayload} from "../../../core/models/recipe-type.model";

@Component({
    selector: 'app-recipe-types-page',
    standalone: true,
    imports: [ReactiveFormsModule, DatePipe, PageHeaderComponent, StatCardComponent, EmptyStateComponent],
    templateUrl: './recipe-types-page.component.html',
    styleUrls: ['./recipe-types-page.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class RecipeTypesPageComponent {

    public constructor(
        private formBuilder: FormBuilder,
        private recipeTypesService: RecipeTypesService,
        private notificationService: NotificationService,
    ) {
        this.loadRecipeTypes();
    }

    readonly loading = signal(true);
    readonly saving = signal(false);
    readonly mode = signal<'create' | 'edit'>('create');
    readonly selectedTypeId = signal<number | null>(null);
    readonly recipeTypes = signal<RecipeType[]>([]);

    readonly form = this.formBuilder.nonNullable.group({
        name: ['', [Validators.required, Validators.maxLength(255)]]
    });

    readonly sortedRecipeTypes = computed(() =>
        [...this.recipeTypes()].sort((left, right) => Date.parse(right.updatedAt) - Date.parse(left.updatedAt))
    );
    readonly activeRecipeTypesCount = computed(() => this.recipeTypes().filter((type) => !type.deletedAt).length);
    readonly archivedRecipeTypesCount = computed(() => this.recipeTypes().filter((type) => !!type.deletedAt).length);

    startCreate(): void {
        this.mode.set('create');
        this.selectedTypeId.set(null);
        this.form.reset({name: ''});
    }

    startEdit(recipeType: RecipeType): void {
        this.mode.set('edit');
        this.selectedTypeId.set(recipeType.id);
        this.form.setValue({name: recipeType.name});
    }

    saveRecipeType(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        const payload = this.form.getRawValue() as SaveRecipeTypePayload;
        const recipeTypeId = this.selectedTypeId();
        const request$ =
            this.mode() === 'edit' && recipeTypeId
                ? this.recipeTypesService.update(recipeTypeId, payload)
                : this.recipeTypesService.create(payload);

        this.saving.set(true);

        request$
            .pipe(finalize(() => this.saving.set(false)))
            .subscribe({
                next: (recipeType) => {
                    if (this.mode() === 'edit') {
                        this.recipeTypes.update((types) => types.map((item) => (item.id === recipeType.id ? recipeType : item)));
                        this.notificationService.success('Categoria atualizada', 'A categoria foi ajustada com sucesso.');
                    } else {
                        this.recipeTypes.update((types) => [recipeType, ...types]);
                        this.notificationService.success('Categoria criada', 'A nova categoria ja esta disponivel para uso.');
                    }

                    this.startCreate();
                }
            });
    }

    archiveRecipeType(recipeType: RecipeType): void {
        if (recipeType.deletedAt) {
            return;
        }

        this.recipeTypesService.delete(recipeType.id).subscribe({
            next: () => {
                this.recipeTypes.update((types) =>
                    types.map((item) =>
                        item.id === recipeType.id
                            ? {
                                ...item,
                                deletedAt: new Date().toISOString(),
                                updatedAt: new Date().toISOString()
                            }
                            : item
                    )
                );

                if (this.selectedTypeId() === recipeType.id) {
                    this.startCreate();
                }

                this.notificationService.success('Categoria arquivada', 'A categoria foi removida da operacao ativa.');
            }
        });
    }

    private loadRecipeTypes(): void {
        this.loading.set(true);

        this.recipeTypesService
            .getAll()
            .pipe(finalize(() => this.loading.set(false)))
            .subscribe({
                next: (recipeTypes) => {
                    this.recipeTypes.set(recipeTypes);
                }
            });
    }
}

export default RecipeTypesPageComponent
