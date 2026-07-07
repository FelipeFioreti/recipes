import {ChangeDetectionStrategy, Component, computed, HostListener, inject, OnInit, signal} from '@angular/core';
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
export class SidebarComponent implements OnInit {
    private readonly mobileBreakpoint = 991;
    readonly menuOpen = signal(true);
    readonly isMobile = signal(false);
    readonly openSubMenus = signal<Set<string>>(new Set());

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
                label: 'Categorias',
                description: 'Categorias',
                route: '/app/categories',
                badge: 'CT',
                icon: 'tags'
            });
        }

        return items;
    });
    readonly authService = inject(AuthService);

    ngOnInit(): void {
        this.syncViewportState();
    }

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

    handleNavigation(): void {
        if (this.isMobile()) {
            this.closeMenu();
        }
    }

    signOut(): void {
        this.closeMenu();
        this.authService.logout();
    }

    @HostListener('window:resize')
    onResize(): void {
        this.syncViewportState();
    }

    private syncViewportState(): void {
        if (typeof window === 'undefined') {
            return;
        }

        const mobile = window.innerWidth <= this.mobileBreakpoint;
        const wasMobile = this.isMobile();
        this.isMobile.set(mobile);

        if (mobile && !wasMobile) {
            this.menuOpen.set(false);
        }

        if (!mobile && wasMobile) {
            this.menuOpen.set(true);
        }
    }
}

