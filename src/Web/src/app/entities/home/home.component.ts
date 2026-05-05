import {ChangeDetectionStrategy, Component} from '@angular/core';
import {RouterLink} from '@angular/router';
import {PageHeaderComponent} from "../../shared/components/page-header/page-header.component";
import {AuthStore} from "../../core/services/auth.store";

@Component({
    selector: 'app-home',
    standalone: true,
    imports: [RouterLink, PageHeaderComponent],
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent {

    public constructor(
        public authStore: AuthStore
    ) {
    }
}
