import {HttpClient, HttpResponse} from '@angular/common/http';
import {inject, Injectable} from '@angular/core';
import {Observable} from 'rxjs';

import {environment} from '../../../environments/environment';
import {createRequestOption} from '../../shared/utils/request-util';
import {IUnit, Unit} from '../models/unit.model';

type EntityArrayResponseType = HttpResponse<IUnit[]>;

@Injectable({providedIn: 'root'})
export class UnitsService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.apiUrl}/unit`;

    getAll(): Observable<Unit[]> {
        return this.http.get<Unit[]>(this.baseUrl);
    }

    getAllPaged(req?: any): Observable<EntityArrayResponseType> {
        const options = createRequestOption(req);

        return this.http.get<IUnit[]>(this.baseUrl, {
            params: options,
            observe: 'response'
        });
    }

    get(id: number | string): Observable<Unit> {
        return this.http.get<Unit>(`${this.baseUrl}/${id}`);
    }

    create(payload: IUnit): Observable<Unit> {
        return this.http.post<Unit>(this.baseUrl, payload);
    }

    update(id: number, payload: IUnit): Observable<Unit> {
        return this.http.put<Unit>(`${this.baseUrl}/${id}`, payload);
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${id}`);
    }
}
