import { ChangeDetectionStrategy, Component, input } from '@angular/core';

type StatCardTone = 'accent' | 'success' | 'neutral';

@Component({
  selector: 'app-stat-card',
  standalone: true,
  templateUrl: './stat-card.component.html',
  styleUrls: ['./stat-card.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class StatCardComponent {
  readonly label = input.required<string>();
  readonly value = input.required<string>();
  readonly helper = input('');
  readonly tone = input<StatCardTone>('neutral');
}
