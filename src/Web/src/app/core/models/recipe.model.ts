import {BaseEntity, IBaseEntity} from "./base-entity.model";
import {IRecipeType} from "./recipe-type.model";
import {IUser} from "./user.model";

export interface IRecipe extends IBaseEntity {
    name?: string;
    description?: string;
    recipeType?: IRecipeType;
    user?: IUser;
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
        public recipeType?: IRecipeType,
        public user?: IUser,
    ) {
        super(id, uuid, createdAt, updatedAt, deletedAt);
    }
}
