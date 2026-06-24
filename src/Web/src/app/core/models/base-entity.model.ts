export interface IBaseEntity {
    id?: number;
    uuid?: string;
    createdAt?: Date;
    updatedAt?: Date;
    deletedAt?: Date | null;
}

export abstract class BaseEntity implements IBaseEntity {
    protected constructor(
        public id?: number,
        public uuid?: string,
        public createdAt?: Date,
        public updatedAt?: Date,
        public deletedAt?: Date | null,
    ) {
    }
}
