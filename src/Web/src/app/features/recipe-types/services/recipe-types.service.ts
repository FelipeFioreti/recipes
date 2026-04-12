import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { RecipeType, SaveRecipeTypePayload } from '../models/recipe-type.model';

@Injectable({ providedIn: 'root' })
export class RecipeTypesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/recipetype`;

  getAll(): Observable<RecipeType[]> {
    return this.http.get<RecipeType[]>(this.baseUrl);
  }

  create(payload: SaveRecipeTypePayload): Observable<RecipeType> {
    return this.http.post<RecipeType>(this.baseUrl, payload);
  }

  update(id: number, payload: SaveRecipeTypePayload): Observable<RecipeType> {
    return this.http.put<RecipeType>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
