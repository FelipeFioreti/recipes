import {Component, DestroyRef, inject, OnInit, signal} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {ActivatedRoute, RouterModule} from '@angular/router';
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {map, switchMap} from "rxjs/operators";
import {CommonModule} from "@angular/common";
import {FontAwesomeModule} from "@fortawesome/angular-fontawesome";
import {TranslateModule} from "@ngx-translate/core";
import {NgSelectModule} from "@ng-select/ng-select";
import {NgbAccordionModule} from "@ng-bootstrap/ng-bootstrap";

import {RecipeActionsService} from "./recipe-actions.service";
import {RecipeService} from "../../../core/services/recipe-service";
import {RecipeTypesService} from "../../../core/services/recipe-types.service";
import {IRecipe, Recipe} from "../../../core/models/recipe.model";
import {IRecipeType} from "../../../core/models/recipe-type.model";

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
        NgbAccordionModule,
        RouterModule
    ]
})
export class RecipeUpdateComponent implements OnInit {
    private readonly fb = inject(FormBuilder);
    private readonly recipeActionsService = inject(RecipeActionsService);
    private readonly recipeService = inject(RecipeService);
    private readonly recipeTypesService = inject(RecipeTypesService);
    private readonly route = inject(ActivatedRoute);
    private readonly destroyRef = inject(DestroyRef);

    recipe = signal<IRecipe | null>(null);
    recipeTypes = signal<IRecipeType[]>([]);
    isSaving = signal(false);

    editForm = this.fb.group({
        Id: [{value: 0, disabled: true}],
        Name: ["", [Validators.required, Validators.minLength(1), Validators.maxLength(255)]],
        Description: ["", [Validators.maxLength(2000)]],
        RecipeType: this.fb.control<IRecipeType | null>(
            null,
            [Validators.required]
        )
    });

    ngOnInit(): void {
        this.recipeTypesService.getAll()
            .pipe(
                takeUntilDestroyed(this.destroyRef),
                map(res => this.recipeTypes.set(res || [])),
                switchMap(() => this.route.data)
            )
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
            RecipeType: recipe.recipeType
        });
    }

    private updateRecipe(recipe: IRecipe): void {
        recipe.name = this.editForm.get(['Name'])!.value;
        recipe.description = this.editForm.get(['Description'])!.value;
        recipe.recipeType = this.editForm.get(['RecipeType'])!.value;
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
