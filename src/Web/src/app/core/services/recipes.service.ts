import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Recipe, SaveRecipePayload } from '../models/recipe.model';
import {environment} from "../../../environments/environment";

@Injectable({ providedIn: 'root' })
export class RecipesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/recipe`;

  getAll(): Observable<Recipe[]> {
    return this.http.get<Recipe[]>(this.baseUrl);
  }

  create(payload: SaveRecipePayload): Observable<Recipe> {
    return this.http.post<Recipe>(this.baseUrl, payload);
  }

  update(id: number, payload: SaveRecipePayload): Observable<Recipe> {
    return this.http.put<Recipe>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}

