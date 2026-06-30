import {DestroyRef, inject, Injectable} from '@angular/core';
import {Router} from '@angular/router';
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {IRecipeType} from "../../../core/models/recipe-type.model";
import {RecipeTypesService} from "../../../core/services/recipe-types.service";
import {NgbModal} from "@ng-bootstrap/ng-bootstrap";
import {TranslateService} from "@ngx-translate/core";
import {
    GenericActionModalComponent
} from "../../../shared/components/generic-action-modal/generic-action-modal.component";

@Injectable({providedIn: 'root'})
export class RecipeTypeActionsService {
    private static readonly FILTER_KEY = 'rec-recipes';

    private readonly destroyRef = inject(DestroyRef);
    private readonly recipeTypeService = inject(RecipeTypesService);
    private readonly router = inject(Router);
    private readonly modalService = inject(NgbModal);
    private readonly translateService = inject(TranslateService);

    delete(recipeType: IRecipeType, successCallback?: Function): void {
        const modalRef = this.modalService.open(GenericActionModalComponent, {
            backdrop: 'static',
            windowClass: 'app-top-modal'
        });

        modalRef.componentInstance.title = this.translateService.instant('entity.delete.title');
        modalRef.componentInstance.message =
            this.translateService.instant('entity.delete.questionWithName', {
                name: recipeType.name || this.translateService.instant('recipeType.detail.title')
            });
        modalRef.componentInstance.actionLabel = this.translateService.instant('entity.action.delete');

        modalRef.componentInstance.confirmed.subscribe(() => {
            this.recipeTypeService
                .delete(recipeType.id!)
                .pipe(takeUntilDestroyed(this.destroyRef))
                .subscribe((result) => successCallback && successCallback(result));
        });
    }

    goToList(): void {
        this.router.navigate(['/app/recipes-types']);
    }

    goToView(recipeType: IRecipeType): void {
        this.router.navigate(['/app/recipes-types', recipeType.id!, 'view']);
    }

    goToViewOrList(recipeType?: IRecipeType): void {
        recipeType && recipeType.id ? this.goToView(recipeType) : this.goToList();
    }

    goToNew(): void {
        this.router.navigate(['/app/recipes-types/new']);
    }

    goToEdit(recipeType: IRecipeType): void {
        this.router.navigate(['/app/recipes-types', recipeType.id!, 'edit']);
    }
}
