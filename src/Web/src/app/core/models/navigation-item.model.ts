export interface NavigationItem {
    label: string;
    description?: string;
    route?: string;
    badge?: string;
    icon?: string;
    children?: NavigationItem[];
}
