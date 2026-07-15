import {Component, DestroyRef, inject, OnInit, signal} from '@angular/core';
import {FormArray, FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {ActivatedRoute} from '@angular/router';
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {catchError} from "rxjs/operators";
import {of} from "rxjs";
import {CommonModule} from "@angular/common";
import {FontAwesomeModule} from "@fortawesome/angular-fontawesome";
import {TranslateModule} from "@ngx-translate/core";
import {NgSelectModule} from "@ng-select/ng-select";

import {RecipeActionsService} from "./recipe-actions.service";
import {RecipeService} from "../../../core/services/recipe-service";
import {CategoriesService} from "../../../core/services/categories.service";
import {UnitsService} from '../../../core/services/units.service';
import {IRecipe, Recipe} from "../../../core/models/recipe.model";
import {ICategory} from "../../../core/models/category.model";
import {IUnit} from '../../../core/models/unit.model';
import {IIngredient} from '../../../core/models/ingredient.model';
import {IStep} from '../../../core/models/step.model';
import {PageHeaderComponent} from "../../../shared/components/page-header/page-header.component";
import {
    EntityAuditAccordionComponent
} from "../../../shared/components/entity-audit-accordion/entity-audit-accordion.component";

@Component({
    selector: 'app-rec-recipe-update',
    templateUrl: './recipe-update.component.html',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FontAwesomeModule,
        TranslateModule,
        NgSelectModule,
        PageHeaderComponent,
        EntityAuditAccordionComponent
    ]
})
export class RecipeUpdateComponent implements OnInit {
    private readonly fb = inject(FormBuilder);
    private readonly recipeActionsService = inject(RecipeActionsService);
    private readonly recipeService = inject(RecipeService);
    private readonly categoriesService = inject(CategoriesService);
    private readonly unitsService = inject(UnitsService);
    private readonly route = inject(ActivatedRoute);
    private readonly destroyRef = inject(DestroyRef);

    recipe = signal<IRecipe | null>(null);
    categories = signal<ICategory[]>([]);
    units = signal<IUnit[]>([]);
    isSaving = signal(false);
    draggedStepIndex = signal<number | null>(null);
    private activeStepPointerId: number | null = null;

    editForm = this.fb.group({
        Id: [{value: 0, disabled: true}],
        Name: ["", [Validators.required, Validators.minLength(1), Validators.maxLength(255)]],
        Description: ["", [Validators.maxLength(2000)]],
        Category: this.fb.control<ICategory | null>(
            null,
            [Validators.required]
        ),
        Ingredients: this.fb.array([]),
        Steps: this.fb.array([]),
    });

    get ingredients(): FormArray {
        return this.editForm.get('Ingredients') as FormArray;
    }

    get steps(): FormArray {
        return this.editForm.get('Steps') as FormArray;
    }

    ngOnInit(): void {
        this.categoriesService.getAll()
            .pipe(
                takeUntilDestroyed(this.destroyRef),
                catchError(() => of([]))
            )
            .subscribe(res => this.categories.set(res || []));

        this.unitsService.getAll()
            .pipe(takeUntilDestroyed(this.destroyRef), catchError(() => of([])))
            .subscribe(res => this.units.set(res || []));

        this.route.data
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe(data => {
                const recipe = data['recipe'] as IRecipe;
                this.bindRecipe(recipe && recipe.id ? recipe : new Recipe());
            });
    }

    private bindRecipe(recipe: IRecipe): void {
        this.recipe.set(recipe);
        this.updateForm(recipe);
    }

    cancel(): void {
        this.recipeActionsService.goToViewOrList(this.recipe()!);
    }

    save(): void {
        const recipe = this.recipe();
        if (!recipe) return;

        this.isSaving.set(true);
        this.updateRecipe(recipe);

        const request = recipe.id
            ? this.recipeService.update(recipe)
            : this.recipeService.create(recipe);

        request.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
            next: (res) => this.onSaveSuccess(res.body!),
            error: () => this.onSaveError()
        });
    }

    private updateForm(recipe: IRecipe): void {
        this.editForm.patchValue({
            Id: recipe.id,
            Name: recipe.name,
            Description: recipe.description,
            Category: recipe.category
        });

        this.ingredients.clear();
        (recipe.ingredients || []).forEach(ingredient => this.addIngredient(ingredient));
        this.steps.clear();
        (recipe.steps || []).sort((a, b) => (a.position || 0) - (b.position || 0))
            .forEach(step => this.addStep(step));
    }

    private updateRecipe(recipe: IRecipe): void {
        recipe.name = this.editForm.get(['Name'])!.value;
        recipe.description = this.editForm.get(['Description'])!.value;
        recipe.category = this.editForm.get(['Category'])!.value ?? undefined;
        recipe.categoryId = recipe.category?.id;
        recipe.ingredients = this.ingredients.getRawValue().map((row: any) => ({
            id: row.Id || undefined,
            name: row.Name,
            quantity: row.Quantity,
            unitId: row.Unit?.id
        }));
        recipe.steps = this.steps.getRawValue().map((row: any, index: number) => ({
            id: row.Id || undefined,
            position: row.Position || index + 1,
            description: row.Description
        }));
    }

    addIngredient(ingredient: IIngredient = {}): void {
        this.ingredients.push(this.fb.group({
            Id: [ingredient.id],
            Name: [ingredient.name || '', [Validators.required, Validators.maxLength(255)]],
            Quantity: [ingredient.quantity ?? null, [Validators.required, Validators.min(0.01)]],
            Unit: [ingredient.unit || null, Validators.required]
        }));
    }

    removeIngredient(index: number): void {
        this.ingredients.removeAt(index);
    }

    addStep(step: IStep = {}): void {
        this.steps.push(this.fb.group({
            Id: [step.id],
            Position: [step.position || this.steps.length + 1],
            Description: [step.description || '', [Validators.required, Validators.maxLength(2000)]]
        }));
    }

    removeStep(index: number): void {
        this.steps.removeAt(index);
        this.normalizeStepPositions();
    }

    startStepDrag(index: number, event: PointerEvent): void {
        if (!event.isPrimary || (event.pointerType === 'mouse' && event.button !== 0))
            return;

        event.preventDefault();
        (event.currentTarget as HTMLElement).setPointerCapture(event.pointerId);
        this.activeStepPointerId = event.pointerId;
        this.draggedStepIndex.set(index);
    }

    moveStepDrag(event: PointerEvent): void {
        if (this.activeStepPointerId !== event.pointerId)
            return;

        event.preventDefault();
        const sourceIndex = this.draggedStepIndex();
        const target = document.elementFromPoint(event.clientX, event.clientY)
            ?.closest<HTMLElement>('[data-step-index]');
        const targetIndex = Number(target?.dataset['stepIndex']);

        if (sourceIndex === null || !Number.isInteger(targetIndex) || sourceIndex === targetIndex)
            return;

        this.moveStep(sourceIndex, targetIndex);
        this.draggedStepIndex.set(targetIndex);
    }

    endStepDrag(event?: PointerEvent): void {
        if (event && this.activeStepPointerId !== event.pointerId)
            return;

        const handle = event?.currentTarget as HTMLElement | null;
        if (event && handle?.hasPointerCapture(event.pointerId))
            handle.releasePointerCapture(event.pointerId);

        this.activeStepPointerId = null;
        this.draggedStepIndex.set(null);
    }

    private normalizeStepPositions(): void {
        this.steps.controls.forEach((step, index) => step.get('Position')?.setValue(index + 1));
    }

    private moveStep(sourceIndex: number, targetIndex: number): void {
        const step = this.steps.at(sourceIndex);
        this.steps.removeAt(sourceIndex);
        this.steps.insert(targetIndex, step);
        this.normalizeStepPositions();
    }

    private onSaveSuccess(recipe: IRecipe): void {
        this.isSaving.set(false);
        this.recipeActionsService.goToView(recipe);
    }

    private onSaveError(): void {
        this.isSaving.set(false);
    }

    isInvalidAndTouched(field: string): boolean {
        const control = this.editForm.get(field);
        return !!control && control.invalid && (control.dirty || control.touched);
    }
}
