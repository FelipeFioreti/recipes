import {DestroyRef, inject, Injectable} from '@angular/core';
import {Router} from '@angular/router';
import {RecipeService} from "../../../core/services/recipe-service";
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {IRecipe} from "../../../core/models/recipe.model";

@Injectable({providedIn: 'root'})
export class RecipeActionsService {
    private static readonly FILTER_KEY = 'rec-recipes';

    private readonly destroyRef = inject(DestroyRef);
    private readonly recipeService = inject(RecipeService);
    private readonly router = inject(Router);

    delete(recipe: IRecipe, successCallback?: Function): void {
        this.recipeService
            .delete(recipe.id!)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe((result) => successCallback && successCallback(result));
    }

    goToList(): void {
        this.router.navigate(['/app/recipes']);
    }

    goToView(recipe: IRecipe): void {
        this.router.navigate(['/app/recipes', recipe.id!, 'view']);
    }

    goToViewOrList(recipe?: IRecipe): void {
        recipe && recipe.id ? this.goToView(recipe) : this.goToList();
    }

    goToNew(): void {
        this.router.navigate(['/app/recipes/new']);
    }

    goToEdit(recipe: IRecipe): void {
        this.router.navigate(['/app/recipes', recipe.id!, 'edit']);
    }
}
