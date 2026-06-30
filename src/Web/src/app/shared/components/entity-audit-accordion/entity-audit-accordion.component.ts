import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { TranslateModule } from '@ngx-translate/core';
import { NgbAccordionModule } from '@ng-bootstrap/ng-bootstrap';

import { IBaseEntity } from '../../../core/models/base-entity.model';

@Component({
    selector: 'app-entity-audit-accordion',
    standalone: true,
    imports: [
        CommonModule,
        FontAwesomeModule,
        TranslateModule,
        NgbAccordionModule
    ],
    templateUrl: './entity-audit-accordion.component.html',
    styleUrls: ['./entity-audit-accordion.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class EntityAuditAccordionComponent {
    readonly entity = input<IBaseEntity | null>(null);
    readonly collapsed = input(true);
}
