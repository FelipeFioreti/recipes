import {HttpClient, HttpResponse} from '@angular/common/http';
import {inject, Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {IRecipeType, RecipeType} from '../models/recipe-type.model';
import {environment} from "../../../environments/environment";
import {createRequestOption} from "../../shared/utils/request-util";

type EntityArrayResponseType = HttpResponse<IRecipeType[]>;

@Injectable({providedIn: 'root'})
export class RecipeTypesService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.apiUrl}/recipeType`;

    getAll(): Observable<RecipeType[]> {
        return this.http.get<RecipeType[]>(this.baseUrl);
    }

    getAllPaged(req?: any): Observable<EntityArrayResponseType> {
        const options = createRequestOption(req);

        return this.http.get<IRecipeType[]>(this.baseUrl, {
            params: options,
            observe: 'response'
        });
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
