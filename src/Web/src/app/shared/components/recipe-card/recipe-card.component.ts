import {ChangeDetectionStrategy, Component, input, output} from '@angular/core';
import {DatePipe} from '@angular/common';
import {IRecipe} from "../../../core/models/recipe.model";

@Component({
    selector: 'app-recipe-card',
    standalone: true,
    imports: [DatePipe],
    templateUrl: './recipe-card.component.html',
    styleUrls: ['./recipe-card.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class RecipeCardComponent {
    readonly recipe = input.required<IRecipe>();
    readonly categoryName = input('Sem categoria');
    readonly selected = input(false);
    readonly editRequested = output<void>();
    readonly deleteRequested = output<void>();

    edit(): void {
        this.editRequested.emit();
    }

    remove(): void {
        this.deleteRequested.emit();
    }
}

