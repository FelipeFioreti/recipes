import {Component, DestroyRef, inject, OnInit} from '@angular/core';
import {HttpHeaders, HttpResponse} from '@angular/common/http';
import {ITEMS_PER_PAGE} from "../../../core/constants/pagination.constants";
import {ActivatedRoute, Router} from "@angular/router";
import {IRecipe} from "../../../core/models/recipe.model";
import {RecipeActionsService} from "./recipe-actions.service";
import {RecipeService} from "../../../core/services/recipeService";
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";


@Component({
    selector: 'app-rec-recipe',
    templateUrl: './recipe.component.html',
    standalone: false,
})
export class RecipeComponent implements OnInit {
    recipes: IRecipe[] | null = null;
    totalItems = 0;
    itemsPerPage = ITEMS_PER_PAGE;
    page!: number;
    predicate!: string;
    ascending!: boolean;
    filter?: Object | null;

    private readonly destroyRef = inject(DestroyRef);

    constructor(
        public recipeActionsService: RecipeActionsService,
        private activatedRoute: ActivatedRoute,
        private recipeService: RecipeService,
        private router: Router,
    ) {
        this.recipes = [];
    }

    ngOnInit(): void {
        this.activatedRoute.data
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe((data) => {

                this.page = data['pagingParams'].page;
                this.ascending = data['pagingParams'].ascending;
                this.predicate = data['pagingParams'].predicate;

                this.loadAll();
            });
    }

    transition(): void {
        this.router.navigate(['./'], {
            relativeTo: this.activatedRoute.parent,
            queryParams: {
                page: this.page,
                sort: this.predicate + ',' + (this.ascending ? 'asc' : 'desc')
            }
        });
        this.loadAll();
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
                page: this.page - 1,
                size: this.itemsPerPage,
                sort: this.sort(),
                ...(this.filter || {})
            })
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe((response: HttpResponse<IRecipe[]>) => {
                this.onSuccess(response.body, response.headers);
            });
    }

    reset(): void {
        this.page = 0;
        this.recipes = [];
        this.loadAll();
    }

    private sort(): string[] {
        const result = [this.predicate + ',' + (this.ascending ? 'asc' : 'desc')];
        if (this.predicate !== 'id') {
            result.push('id');
        }
        return result;
    }

    private onSuccess(recipes: IRecipe[] | null, headers: HttpHeaders): void {
        this.totalItems = Number(headers.get('X-Total-Count'));
        this.recipes = recipes;
    }
}
