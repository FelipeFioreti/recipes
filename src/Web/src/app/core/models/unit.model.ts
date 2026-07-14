import {BaseEntity, IBaseEntity} from './base-entity.model';

export interface IUnit extends IBaseEntity {
    name?: string;
    abbreviation?: string;
}

export class Unit extends BaseEntity implements IUnit {
    constructor(
        id?: number,
        uuid?: string,
        createdAt?: Date,
        updatedAt?: Date,
        deletedAt?: Date | null,
        public name?: string,
        public abbreviation?: string,
    ) {
        super(id, uuid, createdAt, updatedAt, deletedAt);
    }
}
