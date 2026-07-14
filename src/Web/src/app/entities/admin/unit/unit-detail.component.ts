import {Component, DestroyRef, inject, OnInit, signal} from '@angular/core';
import {ActivatedRoute, RouterModule} from '@angular/router';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {CommonModule} from '@angular/common';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';
import {TranslateModule} from '@ngx-translate/core';

import {IUnit} from '../../../core/models/unit.model';
import {PageHeaderComponent} from '../../../shared/components/page-header/page-header.component';
import {EntityAuditAccordionComponent} from '../../../shared/components/entity-audit-accordion/entity-audit-accordion.component';

@Component({
    selector: 'app-unit-detail',
    templateUrl: './unit-detail.component.html',
    standalone: true,
    imports: [
        CommonModule,
        RouterModule,
        FontAwesomeModule,
        TranslateModule,
        PageHeaderComponent,
        EntityAuditAccordionComponent
    ]
})
export class UnitDetailComponent implements OnInit {
    unit = signal<IUnit | null>(null);

    private readonly route = inject(ActivatedRoute);
    private readonly destroyRef = inject(DestroyRef);

    ngOnInit(): void {
        this.route.data.pipe(
            takeUntilDestroyed(this.destroyRef)
        ).subscribe(({unit}) => this.unit.set(unit));
    }

    previousState(): void {
        window.history.back();
    }
}
