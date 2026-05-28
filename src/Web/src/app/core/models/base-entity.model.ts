export interface IBaseEntity {
    Id?: number;
    Uuid?: string;
    CreatedAt?: Date;
    UpdatedAt?: Date;
    DeletedAt?: Date | null;
}

export abstract class BaseEntity implements IBaseEntity {
    protected constructor(
        public Id?: number,
        public Uuid?: string,
        public CreatedAt?: Date,
        public UpdatedAt?: Date,
        public DeletedAt?: Date | null,
    ) {
    }
}
