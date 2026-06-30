import {IconProp} from "@fortawesome/fontawesome-svg-core";

export interface NavigationItem {
    label: string;
    description?: string;
    route?: string;
    badge?: string;
    icon?: IconProp;
}
