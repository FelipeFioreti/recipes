import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-toast-outlet',
  standalone: true,
  templateUrl: './toast-outlet.component.html',
  styleUrls: ['./toast-outlet.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ToastOutletComponent {
  readonly notificationService = inject(NotificationService);

  dismiss(id: number): void {
    this.notificationService.dismiss(id);
  }
}

