import {IBaseEntity} from './base-entity.model';
import {IUnit} from './unit.model';

export interface IIngredient extends IBaseEntity {
    recipeId?: number;
    name?: string;
    quantity?: number;
    unitId?: number;
    unit?: IUnit;
}
