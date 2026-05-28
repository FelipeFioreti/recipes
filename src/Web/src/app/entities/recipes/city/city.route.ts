import {Injectable} from '@angular/core';
import {HttpResponse} from '@angular/common/http';
import {ActivatedRouteSnapshot, Resolve, Router, Routes} from '@angular/router';
import {EMPTY, Observable, of} from 'rxjs';
import {flatMap} from 'rxjs/operators';

import {Authority} from 'app/shared/model/enumerations/authority.enum';
import {UserRouteAccessService} from 'app/core/auth/user-route-access-service';
import {City, ICity} from 'app/shared/model/location/city.model';
import {CityService} from '../../../shared/services/location/city.service';
import {CityComponent} from './city.component';
import {CityDetailComponent} from './city-detail.component';
import {CityUpdateComponent} from './city-update.component';
import {JhiResolvePagingParams} from 'ng-jhipster';
import {FeatureAuthority} from 'app/shared/model/enumerations/feature-authority.enum';

@Injectable({providedIn: 'root'})
export class CityResolve implements Resolve<ICity> {
  constructor(private service: CityService, private router: Router) {}


