import {ChangeDetectionStrategy, Component, computed, inject, signal} from '@angular/core';
import {RouterLink, RouterLinkActive, RouterOutlet} from '@angular/router';
import {NavigationItem} from '../../core/models/navigation-item.model';
import {AuthService} from '../../core/services/auth.service';
import {AuthStore} from '../../core/services/auth.store';

@Component({
    selector: 'app-shell-layout',
    standalone: true,
    imports: [RouterOutlet, RouterLink, RouterLinkActive],
    templateUrl: './common-layout.component.html',
    styleUrls: ['./common-layout.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class CommonLayoutComponent {
    private readonly authService = inject(AuthService);
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
                description: 'Resumo operacional',
                route: '/app/home',
                badge: 'DB',
                icon: 'bx-grid-alt'
            },
            {
                label: 'Receitas',
                description: 'Catalogo e edicao',
                icon: 'bx-collection',
                children: [
                    {label: 'Todas as Receitas', route: '/app/recipes'},
                    {label: 'Nova Receita', route: '/app/recipes/new'}
                ]
            }
        ];

        if (this.authStore.isAdmin()) {
            items.push({
                label: 'Categorias',
                description: 'Curadoria do acervo',
                route: '/app/categories',
                badge: 'CT',
                icon: 'bx-book-alt'
            });
        }

        return items;
    });

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
        this.authService.signOut();
    }
}

