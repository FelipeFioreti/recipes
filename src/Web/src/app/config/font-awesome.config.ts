import {inject, provideAppInitializer} from '@angular/core';
import {FaIconLibrary} from '@fortawesome/angular-fontawesome';

import {fas} from '@fortawesome/free-solid-svg-icons';
import {far} from '@fortawesome/free-regular-svg-icons';
import {fab} from '@fortawesome/free-brands-svg-icons';

export const fontAwesomeProviders = [
    provideAppInitializer(() => {
        const library = inject(FaIconLibrary);

        library.addIconPacks(fas, far, fab);
    })
];