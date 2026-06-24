import {DestroyRef, inject, Injectable} from '@angular/core';
import {Router} from '@angular/router';
import {RecipeService} from "../../../core/services/recipeService";
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {IRecipe} from "../../../core/models/recipe.model";

@Injectable({providedIn: 'root'})
export class RecipeTypeActionsService {
    private static readonly FILTER_KEY = 'rec-recipes';

    private readonly destroyRef = inject(DestroyRef);
    private readonly recipeService = inject(RecipeService);
    private readonly router = inject(Router);

    delete(recipe: IRecipe, successCallback?: Function): void {
        this.recipeService
            .delete(recipe.Id!)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe((result) => successCallback && successCallback(result));
    }

    goToList(): void {
        this.router.navigate(['/rec/recipes']);
    }

    goToView(recipe: IRecipe): void {
        this.router.navigate(['/rec/recipes', recipe.Id!, 'view']);
    }

    goToViewOrList(recipe?: IRecipe): void {
        recipe && recipe.Id ? this.goToView(recipe) : this.goToList();
    }

    goToNew(): void {
        this.router.navigate(['/rec/recipes/new']);
    }

    goToEdit(recipe: IRecipe): void {
        this.router.navigate(['/rec/recipes', recipe.Id!, 'edit']);
    }
}
