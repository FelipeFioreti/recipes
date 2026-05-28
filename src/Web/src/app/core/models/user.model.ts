import {BaseEntity, IBaseEntity} from "./base-entity.model";

export interface IUser extends IBaseEntity {
    Name?: string;
    Email?: string;
}

export class User extends BaseEntity implements IUser {
    constructor(
        Id?: number,
        Uuid?: string,
        CreatedAt?: Date,
        UpdatedAt?: Date,
        DeletedAt?: Date | null,
        public Name?: string,
    ) {
        super(Id, Uuid, CreatedAt, UpdatedAt, DeletedAt);
    }
}
