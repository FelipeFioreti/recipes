import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';

import {FormBuilder, NgForm, Validators} from '@angular/forms';
import {ActivatedRoute} from '@angular/router';
import {Subscription} from 'rxjs';

import {City, ICity} from 'app/shared/model/location/city.model';
import {CityService} from 'app/shared/services/location/city.service';
import {PageService} from 'app/shared/layout/window/page.service';
import {TranslateService} from '@ngx-translate/core';
import {GenericModalService} from 'app/shared/modal/generic-modal.service';
import {CityActionsService} from 'app/entities/location/city/city-actions.service';
import {Page} from 'app/shared/layout/window/page.model';
import {ButtonBuilder} from 'app/shared/layout/window/button-builder';
import {unsubscribe} from 'app/shared/util/react-util';
import {map, switchMap} from 'rxjs/operators';
import {HttpResponse} from '@angular/common/http';
import {IState} from 'app/shared/model/location/state.model';
import {StateService} from 'app/shared/services/location/state.service';

@Component({
    selector: 'app-loc-city-update',
    templateUrl: './recipe-update.component.html'
})
export class CityUpdateComponent implements OnInit, OnDestroy {
    city!: City;
    states: IState[] = [];
    isSaving = false;

    editForm = this.fb.group({
        id: [{value: null, disabled: true}],
        active: [false, [Validators.required]],
        name: [null, [Validators.required, Validators.minLength(1), Validators.maxLength(255)]],
        state: [null, Validators.required]
    });

    @ViewChild('cityForm')
    form?: NgForm;

    private subscriptions: Subscription[] = [];

    constructor(
        private cityService: CityService,
        private stateService: StateService,
        private route: ActivatedRoute,
        private pageService: PageService,
        private cityActionsService: CityActionsService,
        private translateService: TranslateService,
        private genericModalService: GenericModalService,
        private fb: FormBuilder
    ) {
    }

    private static newCity(): ICity {
        const newCity = new City();
        newCity.active = true;
        return newCity;
    }

    ngOnInit(): void {
        this.setupPage();
        this.subscriptions.push(
            this.stateService
                .query()
                .pipe(
                    map((res: HttpResponse<IState[]>) => (this.states = res.body || [])),
                    switchMap(() => this.route.data)
                )
                .subscribe(({city}) => this.bindCity(city && city.id ? city : CityUpdateComponent.newCity()))
        );
    }

    private setupPage(): void {
        this.pageService.setCurrentPage(
            new Page(
                () => 'map-signs',
                () => 'sidebar.admin.city',
                () => (this.city ? this.city.name! : ''),
                () => undefined,
                () => this.editForm.dirty,
                [
                    ButtonBuilder.cancel(() => this.cancel()),
                    ButtonBuilder.save(
                        () => this.promptSave(),
                        () => this.editForm.dirty && this.editForm.valid && !this.isSaving
                    )
                ]
            )
        );
    }

    private bindCity(city: ICity): void {
        this.city = city;
        this.updateForm(this.city);
    }

    private cancel(): void {
        if (!this.editForm.dirty) {
            this.cityActionsService.goToViewOrList(this.city);
            return;
        }

        this.genericModalService.show('entity.modals.cancel.edit.title', 'entity.modals.cancel.edit.question', [
            ButtonBuilder.back(() => this.genericModalService.dismiss()),
            ButtonBuilder.cancel(() => {
                this.cityActionsService.goToViewOrList(this.city);
                this.genericModalService.confirm();
            })
        ]);
    }

    private promptSave(): void {
        if (!this.editForm.valid) return;
        this.genericModalService.show('entity.modals.save.edit.title', 'entity.modals.save.edit.question', [
            ButtonBuilder.cancel(() => this.genericModalService.dismiss()),
            ButtonBuilder.save(() => this.form && this.form.ngSubmit.emit())
        ]);
    }

    save(): void {
        this.isSaving = true;
        this.updateCity(this.city);

        this.subscriptions.push(
            this.city.id !== undefined
                ? this.cityService.update(this.city).subscribe(
                    updatedCity => this.onSaveSuccess(updatedCity.body!),
                    () => this.onSaveError()
                )
                : this.cityService.create(this.city).subscribe(
                    createdCity => this.onSaveSuccess(createdCity.body!),
                    () => this.onSaveError()
                )
        );
    }

    private updateForm(city: ICity): void {
        this.editForm.patchValue({
            id: city.id,
            uuid: city.uuid,
            active: city.active,
            name: city.name,
            state: city.state
        });
    }

    private updateCity(city: ICity): void {
        city.active = this.editForm.get(['active'])!.value;
        city.name = this.editForm.get(['name'])!.value;
        city.state = this.editForm.get(['state'])!.value;
    }

    private onSaveSuccess(city: ICity): void {
        this.genericModalService.confirm();
        this.isSaving = false;
        this.cityActionsService.goToView(city);
    }

    private onSaveError(): void {
        this.genericModalService.dismiss();
        this.isSaving = false;
    }

    isInvalidAndTouched(field: string): boolean {
        const input = this.editForm.get(field);
        return input !== null && input.invalid && (input.dirty || input.touched);
    }

    ngOnDestroy(): void {
        unsubscribe(this.subscriptions);
    }
}
