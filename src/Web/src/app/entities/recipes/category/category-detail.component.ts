import {Component, DestroyRef, inject} from '@angular/core';
import {ActivatedRoute, RouterModule} from '@angular/router';
import {CommonModule} from "@angular/common";
import {FontAwesomeModule} from "@fortawesome/angular-fontawesome";
import {TranslateModule} from "@ngx-translate/core";
import {PageHeaderComponent} from "../../../shared/components/page-header/page-header.component";
import {
    EntityAuditAccordionComponent
} from "../../../shared/components/entity-audit-accordion/entity-audit-accordion.component";
import {AuthService} from "../../../core/services/auth.service";
import {CategoryActionsService} from "./category-actions.service";
import {toSignal} from "@angular/core/rxjs-interop";
import {map} from "rxjs";

@Component({
    selector: 'app-category-detail',
    templateUrl: './category-detail.component.html',
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
export class CategoryDetailComponent {

    private readonly route = inject(ActivatedRoute);
    private readonly destroyRef = inject(DestroyRef);
    readonly authService = inject(AuthService);
    private readonly categoryActionsService = inject(CategoryActionsService);

    category = toSignal(
        this.route.data.pipe(map(({category}) => category)),
        {initialValue: null}
    );

    goToView(): void {
        this.categoryActionsService.goToList();
    }
}
