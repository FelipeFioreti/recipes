import {Component, DestroyRef, inject, signal} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {ActivatedRoute, RouterModule} from '@angular/router';
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {CommonModule} from "@angular/common";
import {FontAwesomeModule} from "@fortawesome/angular-fontawesome";
import {TranslateModule} from "@ngx-translate/core";
import {NgSelectModule} from "@ng-select/ng-select";
import {NgbAccordionModule} from "@ng-bootstrap/ng-bootstrap";

import {RecipeTypeActionsService} from "./recipe-type-actions.service";
import {RecipeService} from "../../../core/services/recipeService";
import {RecipeTypesService} from "../../../core/services/recipe-types.service";
import {IRecipeType, RecipeType} from "../../../core/models/recipe-type.model";

@Component({
    selector: 'app-rec-recipe-type-update',
    templateUrl: './recipe-type-update.component.html',
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
export class RecipeTypeUpdateComponent {
    private readonly fb = inject(FormBuilder);
    private readonly recipeActionsService = inject(RecipeTypeActionsService);
    private readonly recipeService = inject(RecipeService);
    private readonly recipeTypesService = inject(RecipeTypesService);
    private readonly route = inject(ActivatedRoute);
    private readonly destroyRef = inject(DestroyRef);

    recipeType = signal<IRecipeType | null>(null);
    isSaving = signal(false);

    editForm = this.fb.group({
        Id: [{value: 0, disabled: true}],
        Name: ["", [Validators.required, Validators.minLength(1), Validators.maxLength(255)]],
    });

    ngOnInit(): void {
        this.route.data.pipe(
            takeUntilDestroyed(this.destroyRef)
        ).subscribe(data => {
            const recipeType = data['recipeType'] as IRecipeType;
            this.bindRecipe(recipeType && recipeType.Id ? recipeType : new RecipeType());
        });
    }

    private bindRecipe(recipe: IRecipeType): void {
        this.recipeType.set(recipe);
        this.updateForm(recipe);
    }

    cancel(): void {
        this.recipeActionsService.goToViewOrList(this.recipeType()!);
    }

    save(): void {
        const recipeType = this.recipeType();
        if (!recipeType) return;

        this.isSaving.set(true);
        this.updateRecipeType(recipeType);

        const request = recipeType.Id
            ? this.recipeService.update(recipeType)
            : this.recipeService.create(recipeType);

        request.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
            next: (res) => this.onSaveSuccess(res.body!),
            error: () => this.onSaveError()
        });
    }

    private updateForm(recipeType: IRecipeType): void {
        this.editForm.patchValue({
            Id: recipeType.Id,
            Name: recipeType.Name,
        });
    }

    private updateRecipeType(recipeType: IRecipeType): void {
        recipeType.Name = this.editForm.get(['Name'])!.value;
    }

    private onSaveSuccess(recipe: IRecipeType): void {
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
