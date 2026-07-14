import {Component, DestroyRef, inject, OnInit, signal} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {ActivatedRoute} from '@angular/router';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {CommonModule} from '@angular/common';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';
import {TranslateModule} from '@ngx-translate/core';

import {IUnit, Unit} from '../../../core/models/unit.model';
import {UnitsService} from '../../../core/services/units.service';
import {PageHeaderComponent} from '../../../shared/components/page-header/page-header.component';
import {EntityAuditAccordionComponent} from '../../../shared/components/entity-audit-accordion/entity-audit-accordion.component';
import {UnitActionsService} from './unit-actions.service';

@Component({
    selector: 'app-rec-unit-update',
    templateUrl: './unit-update.component.html',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FontAwesomeModule,
        TranslateModule,
        PageHeaderComponent,
        EntityAuditAccordionComponent
    ]
})
export class UnitUpdateComponent implements OnInit {
    private readonly fb = inject(FormBuilder);
    private readonly unitActionsService = inject(UnitActionsService);
    private readonly unitsService = inject(UnitsService);
    private readonly route = inject(ActivatedRoute);
    private readonly destroyRef = inject(DestroyRef);

    unit = signal<IUnit | null>(null);
    isSaving = signal(false);

    editForm = this.fb.group({
        Id: [{value: 0, disabled: true}],
        Name: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(50)]],
        Abbreviation: ['', [Validators.maxLength(10)]],
    });

    ngOnInit(): void {
        this.route.data.pipe(
            takeUntilDestroyed(this.destroyRef)
        ).subscribe(data => {
            const unit = data['unit'] as IUnit;
            this.bindUnit(unit && unit.id ? unit : new Unit());
        });
    }

    private bindUnit(unit: IUnit): void {
        this.unit.set(unit);
        this.updateForm(unit);
    }

    cancel(): void {
        this.unitActionsService.goToViewOrList(this.unit()!);
    }

    save(): void {
        const unit = this.unit();
        if (!unit) return;

        this.isSaving.set(true);
        this.updateUnit(unit);

        const request = unit.id && unit.id > 0
            ? this.unitsService.update(unit.id, unit)
            : this.unitsService.create(unit);

        request.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
            next: (res) => this.onSaveSuccess(res),
            error: () => this.onSaveError()
        });
    }

    private updateForm(unit: IUnit): void {
        this.editForm.patchValue({
            Id: unit.id,
            Name: unit.name,
            Abbreviation: unit.abbreviation,
        });
    }

    private updateUnit(unit: IUnit): void {
        unit.name = this.editForm.get(['Name'])!.value ?? '';
        unit.abbreviation = this.editForm.get(['Abbreviation'])!.value ?? '';
    }

    private onSaveSuccess(unit: IUnit): void {
        this.isSaving.set(false);
        this.unitActionsService.goToView(unit);
    }

    private onSaveError(): void {
        this.isSaving.set(false);
    }

    isInvalidAndTouched(field: string): boolean {
        const control = this.editForm.get(field);
        return !!control && control.invalid && (control.dirty || control.touched);
    }
}
