import {ChangeDetectionStrategy, Component, computed, inject, signal} from '@angular/core';
import {RouterLink, RouterLinkActive, RouterOutlet} from '@angular/router';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';
import {NavigationItem} from '../../core/models/navigation-item.model';
import {AuthService} from '../../core/services/auth.service';

@Component({
    selector: 'app-shell-layout',
    standalone: true,
    imports: [RouterOutlet, RouterLink, RouterLinkActive, FontAwesomeModule],
    templateUrl: './sidebar-component.html',
    styleUrls: ['./sidebar-component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidebarComponent {
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
                icon: 'house'
            },
            {
                label: 'Receitas',
                description: 'Receitas',
                route: '/app/recipes',
                badge: 'RC',
                icon: 'book-open'
            }
        ];

        if (this.authService.isAdmin()) {
            items.push({
                label: 'Tipos de receitas',
                description: 'Tipos de receitas',
                route: '/app/recipes-types',
                badge: 'TP',
                icon: 'tags'
            });
        }

        return items;
    });
    readonly authService = inject(AuthService);

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

