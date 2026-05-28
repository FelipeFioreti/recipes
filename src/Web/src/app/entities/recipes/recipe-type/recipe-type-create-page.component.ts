import {ChangeDetectionStrategy, Component, signal} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {Router, RouterLink} from '@angular/router';
import {finalize} from 'rxjs';
import {SaveRecipeTypePayload} from '../../../core/models/recipe-type.model';
import {NotificationService} from '../../../core/services/notification.service';
import {RecipeTypesService} from '../../../core/services/recipe-types.service';
import {PageHeaderComponent} from '../../../shared/components/page-header/page-header.component';

@Component({
    selector: 'app-recipe-type-create-page',
    standalone: true,
    imports: [ReactiveFormsModule, RouterLink, PageHeaderComponent],
    templateUrl: './recipe-type-create-page.component.html',
    styleUrls: ['./recipe-type-create-page.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class RecipeTypeCreatePageComponent {
    readonly saving = signal(false);

    readonly form = this.formBuilder.nonNullable.group({
        name: ['', [Validators.required, Validators.maxLength(255)]]
    });

    public constructor(
        private formBuilder: FormBuilder,
        private recipeTypesService: RecipeTypesService,
        private notificationService: NotificationService,
        private router: Router
    ) {
    }

    saveRecipeType(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.saving.set(true);
        this.recipeTypesService.create(this.form.getRawValue() as SaveRecipeTypePayload)
            .pipe(finalize(() => this.saving.set(false)))
            .subscribe({
                next: () => {
                    this.notificationService.success('Tipo criado', 'O tipo de receita foi cadastrado com sucesso.');
                    void this.router.navigateByUrl('/app/recipe-types');
                }
            });
    }
}
