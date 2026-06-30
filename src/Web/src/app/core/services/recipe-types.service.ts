import {HttpClient} from '@angular/common/http';
import {inject, Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {IRecipeType, RecipeType} from '../models/recipe-type.model';
import {environment} from "../../../environments/environment";

@Injectable({providedIn: 'root'})
export class RecipeTypesService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.apiUrl}/recipeType`;

    getAll(): Observable<RecipeType[]> {
        return this.http.get<RecipeType[]>(this.baseUrl);
    }

    get(id: number | string): Observable<RecipeType> {
        return this.http.get<RecipeType>(`${this.baseUrl}/${id}`);
    }

    create(payload: IRecipeType): Observable<RecipeType> {
        return this.http.post<RecipeType>(this.baseUrl, payload);
    }

    update(id: number, payload: IRecipeType): Observable<RecipeType> {
        return this.http.put<RecipeType>(`${this.baseUrl}/${id}`, payload);
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${id}`);
    }
}
