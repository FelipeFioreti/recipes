import {HttpClient, HttpResponse} from '@angular/common/http';
import {inject, Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {Category, ICategory} from '../models/category.model';
import {environment} from "../../../environments/environment";
import {createRequestOption} from "../../shared/utils/request-util";

type EntityArrayResponseType = HttpResponse<ICategory[]>;

@Injectable({providedIn: 'root'})
export class CategoriesService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.apiUrl}/category`;

    getAll(): Observable<Category[]> {
        return this.http.get<Category[]>(this.baseUrl);
    }

    getAllPaged(req?: any): Observable<EntityArrayResponseType> {
        const options = createRequestOption(req);

        return this.http.get<ICategory[]>(this.baseUrl, {
            params: options,
            observe: 'response'
        });
    }

    get(id: number | string): Observable<Category> {
        return this.http.get<Category>(`${this.baseUrl}/${id}`);
    }

    create(payload: ICategory): Observable<Category> {
        return this.http.post<Category>(this.baseUrl, payload);
    }

    update(id: number, payload: ICategory): Observable<Category> {
        return this.http.put<Category>(`${this.baseUrl}/${id}`, payload);
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${id}`);
    }
}
