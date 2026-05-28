import {HttpClient, HttpResponse} from '@angular/common/http';
import {inject, Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {IRecipe} from '../models/recipe.model';
import {environment} from "../../../environments/environment";
import {createRequestOption} from "../../shared/utils/request-util";

type EntityResponseType = HttpResponse<IRecipe>;
type EntityArrayResponseType = HttpResponse<IRecipe[]>;

@Injectable({providedIn: 'root'})
export class RecipeService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.apiUrl}/recipe`;

    getAll(req?: any): Observable<EntityArrayResponseType> {
        const options = createRequestOption(req);
        return this.http.get<IRecipe[]>(this.baseUrl, {
            params: options,
            observe: 'response'
        });
    }

    get(id: string): Observable<EntityResponseType> {
        return this.http.get<IRecipe>(`${this.baseUrl}/${id}`, {
            observe: 'response'
        });
    }

    create(recipe: IRecipe): Observable<EntityResponseType> {
        return this.http.post<IRecipe>(this.baseUrl, recipe, {
            observe: 'response'
        });
    }

    update(recipe: IRecipe): Observable<EntityResponseType> {
        return this.http.put<IRecipe>(`${this.baseUrl}`, recipe, {
            observe: 'response'
        });
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${id}`);
    }
}

