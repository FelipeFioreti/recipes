import {GenericModalService} from 'app/shared/modal/generic-modal.service';
import {ButtonBuilder} from 'app/shared/layout/window/button-builder';
import {TranslateService} from '@ngx-translate/core';
import {Injectable, OnDestroy} from '@angular/core';
import {Subscription} from 'rxjs';
import {Router} from '@angular/router';
import {unsubscribe} from 'app/shared/util/react-util';
import {CityService} from 'app/shared/services/location/city.service';
import {City, ICity} from 'app/shared/model/location/city.model';
import {Criteria} from 'app/shared/filter/criteria.model';
import {Authority} from 'app/shared/model/enumerations/authority.enum';
import {FeatureAuthority} from 'app/shared/model/enumerations/feature-authority.enum';
import {AccountService} from 'app/core/auth/account.service';

@Injectable({providedIn: 'root'})
export class CityActionsService implements OnDestroy {
  private subscriptions: Subscription[] = [];
  private static readonly FILTER_KEY = 'loc-cities';

  constructor(
    private cityService: CityService,
    private router: Router,
    private genericModalService: GenericModalService,
    private translateService: TranslateService,
    private accountService: AccountService
  ) {}

  delete(city: ICity, successCallback?: Function): void {
    this.genericModalService.show('entity.delete.title', this.translateService.instant('city.modals.delete.question', {id: city._label}), [
      ButtonBuilder.cancel(() => this.genericModalService.dismiss()),
      ButtonBuilder.delete(() => {
        this.subscriptions.push(this.cityService.delete(city.id!).subscribe(result => successCallback && successCallback(result)));
        this.genericModalService.confirm();
      })
    ]);
  }

  goToList(): void {
    this.router.navigate(['/loc/cities']);
  }

  goToView(city: ICity): void {
    this.router.navigate(['/loc/cities', city.id!, 'view']);
  }

  goToViewOrList(city?: ICity): void {
    city && city.id ? this.goToView(city) : this.goToList();
  }

  goToNew(): void {
    this.router.navigate(['/loc/cities/new']);
  }

  goToEdit(city: ICity): void {
    this.router.navigate(['/loc/cities', city.id!, 'edit']);
  }

  hasPrivilegeToView(): boolean {
    return this.accountService.hasAnyAuthority([Authority.SYSTEM_ADMIN, FeatureAuthority.LOC_CITY_VIEW]);
  }

  hasPrivilegeToCreate(): boolean {
    return this.accountService.hasAnyAuthority([Authority.SYSTEM_ADMIN, FeatureAuthority.LOC_CITY_CREATE]);
  }

  hasPrivilegeToEdit(): boolean {
    return this.accountService.hasAnyAuthority([Authority.SYSTEM_ADMIN, FeatureAuthority.LOC_CITY_EDIT]);
  }

  hasPrivilegeToDelete(): boolean {
    return this.accountService.hasAnyAuthority([Authority.SYSTEM_ADMIN, FeatureAuthority.LOC_CITY_DELETE]);
  }

  saveCriterias(criterias: Criteria<City>[]): void {
    Criteria.saveCriterias(CityActionsService.FILTER_KEY, criterias);
  }

  restoreCriterias(criterias: Criteria<City>[]): void {
    Criteria.restoreCriterias(CityActionsService.FILTER_KEY, criterias);
  }

  saveFilter(filter: Object | null): void {
    Criteria.saveFilter(CityActionsService.FILTER_KEY, filter);
  }

  getSavedFilter(): Object | null {
    return Criteria.getSavedFilter(CityActionsService.FILTER_KEY);
  }

  ngOnDestroy(): void {
    unsubscribe(this.subscriptions);
  }
}
