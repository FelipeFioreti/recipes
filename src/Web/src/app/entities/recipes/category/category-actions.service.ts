import {DestroyRef, inject, Injectable} from '@angular/core';
import {Router} from '@angular/router';
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {ICategory} from "../../../core/models/category.model";
import {CategoriesService} from "../../../core/services/categories.service";
import {NgbModal} from "@ng-bootstrap/ng-bootstrap";
import {TranslateService} from "@ngx-translate/core";
import {
    GenericActionModalComponent
} from "../../../shared/components/generic-action-modal/generic-action-modal.component";

@Injectable({providedIn: 'root'})
export class CategoryActionsService {
    private readonly destroyRef = inject(DestroyRef);
    private readonly categoriesService = inject(CategoriesService);
    private readonly router = inject(Router);
    private readonly modalService = inject(NgbModal);
    private readonly translateService = inject(TranslateService);

    delete(category: ICategory, successCallback?: Function): void {
        const modalRef = this.modalService.open(GenericActionModalComponent, {
            backdrop: 'static',
            windowClass: 'app-top-modal'
        });

        modalRef.componentInstance.title = this.translateService.instant('entity.delete.title');
        modalRef.componentInstance.message =
            this.translateService.instant('entity.delete.questionWithName', {
                name: category.name || this.translateService.instant('category.detail.title')
            });
        modalRef.componentInstance.actionLabel = this.translateService.instant('entity.action.delete');

        modalRef.componentInstance.confirmed.subscribe(() => {
            this.categoriesService
                .delete(category.id!)
                .pipe(takeUntilDestroyed(this.destroyRef))
                .subscribe((result) => successCallback && successCallback(result));
        });
    }

    goToList(): void {
        this.router.navigate(['/app/categories']);
    }

    goToView(category: ICategory): void {
        this.router.navigate(['/app/categories', category.id!, 'view']);
    }

    goToViewOrList(category?: ICategory): void {
        category && category.id ? this.goToView(category) : this.goToList();
    }

    goToNew(): void {
        this.router.navigate(['/app/categories/new']);
    }

    goToEdit(category: ICategory): void {
        this.router.navigate(['/app/categories', category.id!, 'edit']);
    }
}
