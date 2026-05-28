import {Subscription} from "rxjs";

export const unsubscribe = (subscriptions: Subscription[]) => {
    subscriptions.forEach(s => s.unsubscribe());
}