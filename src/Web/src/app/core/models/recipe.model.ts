import {BaseEntity, IBaseEntity} from "./base-entity.model";
import {IRecipeType} from "./recipe-type.model";
import {IUser} from "./user.model";

export interface IRecipe extends IBaseEntity {
    Name?: string;
    Description?: string;
    RecipeType?: IRecipeType;
    User?: IUser;
}

export class Recipe extends BaseEntity implements IRecipe {
    constructor(
        Id?: number,
        Uuid?: string,
        CreatedAt?: Date,
        UpdatedAt?: Date,
        DeletedAt?: Date | null,
        public Name?: string,
        public Description?: string,
        public RecipeType?: IRecipeType,
        public User?: IUser,
    ) {
        super(Id, Uuid, CreatedAt, UpdatedAt, DeletedAt);
    }
}
