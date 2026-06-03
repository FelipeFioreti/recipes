import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { HttpHeaders, HttpResponse } from '@angular/common/http';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { TranslateModule } from '@ngx-translate/core';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';

import { ITEMS_PER_PAGE } from '../../../core/constants/pagination.constants';
import { IRecipe } from '../../../core/models/recipe.model';
import { RecipeActionsService } from './recipe-actions.service';
import { RecipeService } from '../../../core/services/recipeService';

@Component({
  selector: 'app-rec-recipe',
  templateUrl: './recipe.component.html',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FontAwesomeModule,
    TranslateModule,
    NgbDropdownModule,
  ],
})
export class RecipeComponent implements OnInit {
  recipes = signal<IRecipe[]>([]);
  totalItems = signal(0);
  itemsPerPage = ITEMS_PER_PAGE;
  page = signal(1);
  filter = signal<any>(null);

  private readonly destroyRef = inject(DestroyRef);
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly recipeService = inject(RecipeService);
  public readonly recipeActionsService = inject(RecipeActionsService);

  ngOnInit(): void {
    this.activatedRoute.data
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((data) => {
        const { page } = data['pagingParams'];
        this.page.set(page);
        this.loadAll();
      });
  }

  goToView(recipe: IRecipe): void {
    this.recipeActionsService.goToView(recipe);
  }

  goToEdit(recipe: IRecipe): void {
    this.recipeActionsService.goToEdit(recipe);
  }

  delete(recipe: IRecipe): void {
    this.recipeActionsService.delete(recipe, () => this.loadAll());
  }

  private loadAll(): void {
    this.recipeService
      .getAll({
        page: this.page() - 1,
        size: this.itemsPerPage,
        ...(this.filter() || {}),
      })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((response: HttpResponse<IRecipe[]>) => {
        this.onSuccess(response.body, response.headers);
      });
  }

  private onSuccess(recipes: IRecipe[] | null, headers: HttpHeaders): void {
    this.totalItems.set(Number(headers.get('X-Total-Count')));
    this.recipes.set(recipes ?? []);
  }
}
