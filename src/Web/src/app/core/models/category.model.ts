import {BaseEntity, IBaseEntity} from "./base-entity.model";

export interface ICategory extends IBaseEntity {
    name?: string;
}

export class Category extends BaseEntity implements ICategory {
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
