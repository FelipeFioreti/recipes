import {Component, DestroyRef, inject, OnInit, signal} from '@angular/core';
import {RouterModule} from '@angular/router';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {CommonModule} from '@angular/common';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';
import {TranslateModule} from '@ngx-translate/core';

import {IRecipeType} from '../../../core/models/recipe-type.model';
import {RecipeTypeActionsService} from './recipe-type-actions.service';
import {RecipeTypesService} from '../../../core/services/recipe-types.service';
import {PageHeaderComponent} from '../../../shared/components/page-header/page-header.component';

@Component({
    selector: 'app-rec-recipe',
    templateUrl: './recipe-type.component.html',
    standalone: true,
    imports: [
        CommonModule,
        RouterModule,
        FontAwesomeModule,
        TranslateModule,
        PageHeaderComponent,
    ],
})
export class RecipeTypeComponent implements OnInit {
    recipeTypes = signal<IRecipeType[]>([]);

    private readonly destroyRef = inject(DestroyRef);
    private readonly recipeTypeService = inject(RecipeTypesService);
    public readonly recipeActionsService = inject(RecipeTypeActionsService);

    ngOnInit(): void {
        this.loadAll();
    }

    goToView(recipeType: IRecipeType): void {
        this.recipeActionsService.goToView(recipeType);
    }

    goToEdit(recipeType: IRecipeType): void {
        this.recipeActionsService.goToEdit(recipeType);
    }

    delete(recipeType: IRecipeType): void {
        this.recipeActionsService.delete(recipeType, () => this.loadAll());
    }

    private loadAll(): void {
        this.recipeTypeService
            .getAll()
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe((recipeTypes) => this.recipeTypes.set(recipeTypes ?? []));
    }
}
