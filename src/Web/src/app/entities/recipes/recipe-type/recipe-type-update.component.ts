import {Component, DestroyRef, inject, OnInit, signal} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {ActivatedRoute} from '@angular/router';
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {CommonModule} from "@angular/common";
import {FontAwesomeModule} from "@fortawesome/angular-fontawesome";
import {TranslateModule} from "@ngx-translate/core";

import {RecipeTypeActionsService} from "./recipe-type-actions.service";
import {RecipeTypesService} from "../../../core/services/recipe-types.service";
import {IRecipeType, RecipeType} from "../../../core/models/recipe-type.model";
import {PageHeaderComponent} from "../../../shared/components/page-header/page-header.component";
import {
    EntityAuditAccordionComponent
} from "../../../shared/components/entity-audit-accordion/entity-audit-accordion.component";

@Component({
    selector: 'app-rec-recipe-type-update',
    templateUrl: './recipe-type-update.component.html',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FontAwesomeModule,
        TranslateModule,
        PageHeaderComponent,
        EntityAuditAccordionComponent
    ]
})
export class RecipeTypeUpdateComponent implements OnInit {
    private readonly fb = inject(FormBuilder);
    private readonly recipeActionsService = inject(RecipeTypeActionsService);
    private readonly recipeTypeService = inject(RecipeTypesService);
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
            this.bindRecipe(recipeType && recipeType.id ? recipeType : new RecipeType());
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

        const request = recipeType.id && recipeType.id > 0
            ? this.recipeTypeService.update(recipeType.id, recipeType)
            : this.recipeTypeService.create(recipeType);

        request.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
            next: (res) => this.onSaveSuccess(res),
            error: () => this.onSaveError()
        });
    }

    private updateForm(recipeType: IRecipeType): void {
        this.editForm.patchValue({
            Id: recipeType.id,
            Name: recipeType.name,
        });
    }

    private updateRecipeType(recipeType: IRecipeType): void {
        recipeType.name = this.editForm.get(['Name'])!.value;
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
