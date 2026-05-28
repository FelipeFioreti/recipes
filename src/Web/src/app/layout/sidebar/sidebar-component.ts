import {ChangeDetectionStrategy, Component, computed, inject, signal} from '@angular/core';
import {RouterLink, RouterLinkActive, RouterOutlet} from '@angular/router';
import {NavigationItem} from '../../core/models/navigation-item.model';
import {AuthService} from '../../core/services/auth.service';
import {AuthStore} from '../../core/services/auth.store';

@Component({
    selector: 'app-shell-layout',
    standalone: true,
    imports: [RouterOutlet, RouterLink, RouterLinkActive],
    templateUrl: './sidebar-component.html',
    styleUrls: ['./sidebar-component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidebarComponent {
    readonly authStore = inject(AuthStore);
    readonly menuOpen = signal(true);
    readonly openSubMenus = signal<Set<string>>(new Set());
    readonly currentDateLabel = new Intl.DateTimeFormat('pt-BR', {
        weekday: 'long',
        day: '2-digit',
        month: 'long'
    }).format(new Date());
    readonly navigation = computed<NavigationItem[]>(() => {
        const items: NavigationItem[] = [
            {
                label: 'Inicio',
                description: 'Tela inicial',
                route: '/app/home',
                badge: 'HM',
                icon: 'bx-grid-alt'
            },
            {
                label: 'Receitas',
                description: 'Receitas',
                route: '/app/recipes',
                badge: 'RC',
                icon: 'bx-collection'
            }
        ];

        if (this.authStore.isAdmin()) {
            items.push({
                label: 'Tipos de receitas',
                description: 'Tipos de receitas',
                route: '/app/recipe-types',
                badge: 'TP',
                icon: 'bx-book-alt'
            });
        }

        return items;
    });
    private readonly authService = inject(AuthService);

    toggleMenu(): void {
        this.menuOpen.update((value) => !value);
    }

    toggleSubMenu(label: string): void {
        this.openSubMenus.update(set => {
            const newSet = new Set(set);
            if (newSet.has(label)) {
                newSet.delete(label);
            } else {
                newSet.add(label);
            }
            return newSet;
        });
    }

    isSubMenuOpen(label: string): boolean {
        return this.openSubMenus().has(label);
    }

    closeMenu(): void {
        this.menuOpen.set(false);
    }

    signOut(): void {
        this.closeMenu();
        this.authService.logout();
    }
}

