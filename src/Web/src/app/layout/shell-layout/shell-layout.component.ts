import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { NavigationItem } from '../../core/models/navigation-item.model';
import { AuthService } from '../../core/services/auth.service';
import { AuthStore } from '../../core/services/auth.store';

@Component({
  selector: 'app-shell-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './shell-layout.component.html',
  styleUrls: ['./shell-layout.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ShellLayoutComponent {
  private readonly authService = inject(AuthService);
  readonly authStore = inject(AuthStore);
  readonly menuOpen = signal(false);
  readonly currentDateLabel = new Intl.DateTimeFormat('pt-BR', {
    weekday: 'long',
    day: '2-digit',
    month: 'long'
  }).format(new Date());

  readonly navigation = computed<NavigationItem[]>(() => {
    const items: NavigationItem[] = [
      {
        label: 'Dashboard',
        description: 'Resumo operacional',
        route: '/app/dashboard',
        badge: 'DB'
      },
      {
        label: 'Receitas',
        description: 'Catalogo e edicao',
        route: '/app/recipes',
        badge: 'RC'
      }
    ];

    if (this.authStore.isAdmin()) {
      items.push({
        label: 'Categorias',
        description: 'Curadoria do acervo',
        route: '/app/categories',
        badge: 'CT'
      });
    }

    return items;
  });

  toggleMenu(): void {
    this.menuOpen.update((value) => !value);
  }

  closeMenu(): void {
    this.menuOpen.set(false);
  }

  signOut(): void {
    this.closeMenu();
    this.authService.signOut();
  }
}

