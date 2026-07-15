import {IBaseEntity} from './base-entity.model';

export interface IStep extends IBaseEntity {
    recipeId?: number;
    position?: number;
    description?: string;
}
