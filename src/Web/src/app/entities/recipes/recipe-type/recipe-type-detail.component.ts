import {Component, DestroyRef, inject, OnInit, signal} from '@angular/core';
import {ActivatedRoute, RouterModule} from '@angular/router';
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {CommonModule} from "@angular/common";
import {FontAwesomeModule} from "@fortawesome/angular-fontawesome";
import {TranslateModule} from "@ngx-translate/core";
import {NgbAccordionModule} from "@ng-bootstrap/ng-bootstrap";
import {IRecipeType} from "../../../core/models/recipe-type.model";

@Component({
    selector: 'app-recipe-type-detail',
    templateUrl: './recipe-type-detail.component.html',
    standalone: true,
    imports: [
        CommonModule,
        RouterModule,
        FontAwesomeModule,
        TranslateModule,
        NgbAccordionModule
    ]
})
export class RecipeTypeDetailComponent implements OnInit {
    recipeType = signal<IRecipeType | null>(null);

    private readonly route = inject(ActivatedRoute);
    private readonly destroyRef = inject(DestroyRef);

    ngOnInit(): void {
        this.route.data.pipe(
            takeUntilDestroyed(this.destroyRef)
        ).subscribe(({recipeType}) => this.recipeType.set(recipeType));
    }

    previousState(): void {
        window.history.back();
    }
}
