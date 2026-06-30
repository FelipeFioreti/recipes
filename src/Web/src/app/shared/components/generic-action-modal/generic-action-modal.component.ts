import { ChangeDetectionStrategy, Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-generic-action-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './generic-action-modal.component.html',
  styleUrls: ['./generic-action-modal.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GenericActionModalComponent {
  @Input() title = '';
  @Input() message = '';
  @Input() actionLabel = '';
  @Input() cancelLabel = 'Cancelar';

  @Output() readonly confirmed = new EventEmitter<void>();

  readonly activeModal = inject(NgbActiveModal);

  cancel(): void {
    this.activeModal.dismiss('cancel');
  }

  confirm(): void {
    this.confirmed.emit();
    this.activeModal.close(true);
  }
}
