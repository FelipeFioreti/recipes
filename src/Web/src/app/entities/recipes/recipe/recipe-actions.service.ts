import {DestroyRef, inject, Injectable} from '@angular/core';
import {Router} from '@angular/router';
import {RecipeService} from "../../../core/services/recipe-service";
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {IRecipe} from "../../../core/models/recipe.model";
import {NgbModal} from "@ng-bootstrap/ng-bootstrap";
import {TranslateService} from "@ngx-translate/core";
import {
    GenericActionModalComponent
} from "../../../shared/components/generic-action-modal/generic-action-modal.component";

@Injectable({providedIn: 'root'})
export class RecipeActionsService {
    private static readonly FILTER_KEY = 'rec-recipes';

    private readonly destroyRef = inject(DestroyRef);
    private readonly recipeService = inject(RecipeService);
    private readonly router = inject(Router);
    private readonly modalService = inject(NgbModal);
    private readonly translateService = inject(TranslateService);

    delete(recipe: IRecipe, successCallback?: Function): void {
        const modalRef = this.modalService.open(GenericActionModalComponent, {
            backdrop: 'static',
            windowClass: 'app-top-modal'
        });

        modalRef.componentInstance.title = this.translateService.instant('entity.delete.title');
        modalRef.componentInstance.message = 
            this.translateService.instant('entity.delete.questionWithName', {
                name: recipe.name || this.translateService.instant('recipe.detail.title')
            });
        modalRef.componentInstance.actionLabel = this.translateService.instant('entity.action.delete');

        modalRef.componentInstance.confirmed.subscribe(() => {
            this.recipeService
                .delete(recipe.id!)
                .pipe(takeUntilDestroyed(this.destroyRef))
                .subscribe((result) => successCallback && successCallback(result));
        });
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
