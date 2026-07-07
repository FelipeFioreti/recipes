import {Component, DestroyRef, inject, OnInit, signal} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {ActivatedRoute} from '@angular/router';
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {CommonModule} from "@angular/common";
import {FontAwesomeModule} from "@fortawesome/angular-fontawesome";
import {TranslateModule} from "@ngx-translate/core";

import {CategoryActionsService} from "./category-actions.service";
import {CategoriesService} from "../../../core/services/categories.service";
import {Category, ICategory} from "../../../core/models/category.model";
import {PageHeaderComponent} from "../../../shared/components/page-header/page-header.component";
import {
    EntityAuditAccordionComponent
} from "../../../shared/components/entity-audit-accordion/entity-audit-accordion.component";

@Component({
    selector: 'app-rec-category-update',
    templateUrl: './category-update.component.html',
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
export class CategoryUpdateComponent implements OnInit {
    private readonly fb = inject(FormBuilder);
    private readonly categoryActionsService = inject(CategoryActionsService);
    private readonly categoriesService = inject(CategoriesService);
    private readonly route = inject(ActivatedRoute);
    private readonly destroyRef = inject(DestroyRef);

    category = signal<ICategory | null>(null);
    isSaving = signal(false);

    editForm = this.fb.group({
        Id: [{value: 0, disabled: true}],
        Name: ["", [Validators.required, Validators.minLength(1), Validators.maxLength(255)]],
    });

    ngOnInit(): void {
        this.route.data.pipe(
            takeUntilDestroyed(this.destroyRef)
        ).subscribe(data => {
            const category = data['category'] as ICategory;
            this.bindCategory(category && category.id ? category : new Category());
        });
    }

    private bindCategory(category: ICategory): void {
        this.category.set(category);
        this.updateForm(category);
    }

    cancel(): void {
        this.categoryActionsService.goToViewOrList(this.category()!);
    }

    save(): void {
        const category = this.category();
        if (!category) return;

        this.isSaving.set(true);
        this.updateCategory(category);

        const request = category.id && category.id > 0
            ? this.categoriesService.update(category.id, category)
            : this.categoriesService.create(category);

        request.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
            next: (res) => this.onSaveSuccess(res),
            error: () => this.onSaveError()
        });
    }

    private updateForm(category: ICategory): void {
        this.editForm.patchValue({
            Id: category.id,
            Name: category.name,
        });
    }

    private updateCategory(category: ICategory): void {
        category.name = this.editForm.get(['Name'])!.value;
    }

    private onSaveSuccess(category: ICategory): void {
        this.isSaving.set(false);
        this.categoryActionsService.goToView(category);
    }

    private onSaveError(): void {
        this.isSaving.set(false);
    }

    isInvalidAndTouched(field: string): boolean {
        const control = this.editForm.get(field);
        return !!control && control.invalid && (control.dirty || control.touched);
    }
}
