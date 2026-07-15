import {Routes} from "@angular/router";

export const ADMIN_ROUTES: Routes = [
    {
        path: 'units',
        loadChildren: () =>
            import('../admin/unit/unit.route').then((m) => m.unitRoute)
    }
];