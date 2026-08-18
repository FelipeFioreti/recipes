import {Component, DestroyRef, inject, OnInit, signal} from '@angular/core';
import {ActivatedRoute, RouterModule} from '@angular/router';
import {IRecipe} from "../../../core/models/recipe.model";
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {CommonModule} from "@angular/common";
import {FontAwesomeModule} from "@fortawesome/angular-fontawesome";
import {TranslateModule} from "@ngx-translate/core";
import {PageHeaderComponent} from "../../../shared/components/page-header/page-header.component";
import {
    EntityAuditAccordionComponent
} from "../../../shared/components/entity-audit-accordion/entity-audit-accordion.component";
import {AuthService} from "../../../core/services/auth.service";
import {RecipeActionsService} from "./recipe-actions.service";

@Component({
    selector: 'app-recipe-detail',
    templateUrl: './recipe-detail.component.html',
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
export class RecipeDetailComponent implements OnInit {

    private readonly route = inject(ActivatedRoute);
    private readonly destroyRef = inject(DestroyRef);
    private readonly authService = inject(AuthService);
    readonly recipeActionsService = inject(RecipeActionsService);

    recipe = signal<IRecipe | null>(null);
    isAdmin = signal<boolean>(this.authService.isAdmin());

    ngOnInit(): void {
        this.route.data.pipe(
            takeUntilDestroyed(this.destroyRef)
        ).subscribe(({recipe}) => this.recipe.set(recipe));
    }
}
