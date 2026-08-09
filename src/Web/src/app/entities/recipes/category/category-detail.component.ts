import {Component, DestroyRef, inject, OnInit, signal} from '@angular/core';
import {ActivatedRoute, RouterModule} from '@angular/router';
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {CommonModule} from "@angular/common";
import {FontAwesomeModule} from "@fortawesome/angular-fontawesome";
import {TranslateModule} from "@ngx-translate/core";
import {ICategory} from "../../../core/models/category.model";
import {PageHeaderComponent} from "../../../shared/components/page-header/page-header.component";
import {
    EntityAuditAccordionComponent
} from "../../../shared/components/entity-audit-accordion/entity-audit-accordion.component";
import {AuthService} from "../../../core/services/auth.service";

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
export class CategoryDetailComponent implements OnInit {

    private readonly route = inject(ActivatedRoute);
    private readonly destroyRef = inject(DestroyRef);
    private authService = inject(AuthService);

    category = signal<ICategory | null>(null);
    isAdmin = signal<boolean>(this.authService.isAdmin());


    ngOnInit(): void {
        this.route.data.pipe(
            takeUntilDestroyed(this.destroyRef)
        ).subscribe(({category}) => this.category.set(category));
    }

    previousState(): void {
        window.history.back();
    }
}
