import {BaseEntity, IBaseEntity} from "./base-entity.model";
import {ICategory} from "./category.model";
import {IUser} from "./user.model";
import {IIngredient} from './ingredient.model';
import {IStep} from './step.model';

export interface IRecipe extends IBaseEntity {
    name?: string;
    description?: string;
    categoryId?: number;
    category?: ICategory;
    user?: IUser;
    ingredients?: IIngredient[];
    steps?: IStep[];
}

export class Recipe extends BaseEntity implements IRecipe {
    constructor(
        id?: number,
        uuid?: string,
        createdAt?: Date,
        updatedAt?: Date,
        deletedAt?: Date | null,
        public name?: string,
        public description?: string,
        public categoryId?: number,
        public category?: ICategory,
        public user?: IUser,
        public ingredients?: IIngredient[],
        public steps?: IStep[],
    ) {
        super(id, uuid, createdAt, updatedAt, deletedAt);
    }
}
