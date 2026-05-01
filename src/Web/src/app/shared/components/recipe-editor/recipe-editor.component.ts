import {ChangeDetectionStrategy, Component, input, output} from '@angular/core';
import {ReactiveFormsModule, FormGroup} from '@angular/forms';
import {RecipeType} from "../../../core/models/recipe-type.model";

@Component({
    selector: 'app-recipe-editor',
    standalone: true,
    imports: [ReactiveFormsModule],
    templateUrl: './recipe-editor.component.html',
    styleUrls: ['./recipe-editor.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class RecipeEditorComponent {
    readonly form = input.required<FormGroup>();
    readonly recipeTypes = input<RecipeType[]>([]);
    readonly saving = input(false);
    readonly mode = input<'create' | 'edit'>('create');
    readonly categoriesUnavailableMessage = input('');
    readonly submitted = output<void>();
    readonly canceled = output<void>();

    submit(): void {
        this.submitted.emit();
    }

    cancel(): void {
        this.canceled.emit();
    }
}

