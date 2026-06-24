import {BaseEntity, IBaseEntity} from "./base-entity.model";

export interface IRecipeType extends IBaseEntity {
    name?: string;
}

export class RecipeType extends BaseEntity implements IRecipeType {
    constructor(
        id?: number,
        uuid?: string,
        createdAt?: Date,
        updatedAt?: Date,
        deletedAt?: Date | null,
        public name?: string,
    ) {
        super(id, uuid, createdAt, updatedAt, deletedAt);
    }
}
