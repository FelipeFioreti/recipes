import {DestroyRef, inject, Injectable} from '@angular/core';
import {Router} from '@angular/router';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {TranslateService} from '@ngx-translate/core';

import {IUnit} from '../../../core/models/unit.model';
import {UnitsService} from '../../../core/services/units.service';
import {GenericActionModalComponent} from '../../../shared/components/generic-action-modal/generic-action-modal.component';

@Injectable({providedIn: 'root'})
export class UnitActionsService {
    private readonly destroyRef = inject(DestroyRef);
    private readonly unitsService = inject(UnitsService);
    private readonly router = inject(Router);
    private readonly modalService = inject(NgbModal);
    private readonly translateService = inject(TranslateService);

    delete(unit: IUnit, successCallback?: Function): void {
        const modalRef = this.modalService.open(GenericActionModalComponent, {
            backdrop: 'static',
            windowClass: 'app-top-modal'
        });

        modalRef.componentInstance.title = this.translateService.instant('entity.delete.title');
        modalRef.componentInstance.message =
            this.translateService.instant('entity.delete.questionWithName', {
                name: unit.name || this.translateService.instant('unit.detail.title')
            });
        modalRef.componentInstance.actionLabel = this.translateService.instant('entity.action.delete');

        modalRef.componentInstance.confirmed.subscribe(() => {
            this.unitsService
                .delete(unit.id!)
                .pipe(takeUntilDestroyed(this.destroyRef))
                .subscribe((result) => successCallback && successCallback(result));
        });
    }

    goToList(): void {
        this.router.navigate(['/app/units']);
    }

    goToView(unit: IUnit): void {
        this.router.navigate(['/app/units', unit.id!, 'view']);
    }

    goToViewOrList(unit?: IUnit): void {
        unit && unit.id ? this.goToView(unit) : this.goToList();
    }

    goToNew(): void {
        this.router.navigate(['/app/units/new']);
    }

    goToEdit(unit: IUnit): void {
        this.router.navigate(['/app/units', unit.id!, 'edit']);
    }
}
