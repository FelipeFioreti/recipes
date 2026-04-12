import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToastOutletComponent } from './shared/components/toast-outlet/toast-outlet.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ToastOutletComponent],
  template: `
    <div class="app-shell">
      <router-outlet></router-outlet>
      <app-toast-outlet></app-toast-outlet>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent {}
