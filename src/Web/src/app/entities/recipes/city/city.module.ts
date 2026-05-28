import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router';

import {AppSharedModule} from 'app/shared/shared.module';
import {CityComponent} from './city.component';
import {CityDetailComponent} from './city-detail.component';
import {CityUpdateComponent} from './city-update.component';
import {cityRoute} from './city.route';

@NgModule({
  imports: [AppSharedModule, RouterModule.forChild(cityRoute)],
  declarations: [CityComponent, CityDetailComponent, CityUpdateComponent],
  entryComponents: []
})
export class AppCityModule {}
