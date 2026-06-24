import {ChangeDetectionStrategy, Component} from '@angular/core';
import {PageHeaderComponent} from "../../shared/components/page-header/page-header.component";
import {AuthService} from "../../core/services/auth.service";

@Component({
    selector: 'app-home',
    standalone: true,
    imports: [PageHeaderComponent],
    templateUrl: './home.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent {

    public constructor(
        public authService: AuthService
    ) {
    }
}
