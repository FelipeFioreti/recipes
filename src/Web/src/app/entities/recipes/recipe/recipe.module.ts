import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { TranslateModule } from '@ngx-translate/core';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgbAccordionModule } from '@ng-bootstrap/ng-bootstrap';

import { RecipeComponent } from './recipe.component';
import { RecipeDetailComponent } from './recipe-detail.component';
import { RecipeUpdateComponent } from './recipe-update.component';
import { recipeRoute } from "./recipe.route";

@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FontAwesomeModule,
        TranslateModule,
        NgSelectModule,
        NgbAccordionModule,
        RouterModule.forChild(recipeRoute),
        RecipeComponent,
        RecipeDetailComponent,
        RecipeUpdateComponent
    ],
})
export class AppRecipeModule {
}
