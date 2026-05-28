import {JwtPayload} from 'jwt-decode';
import {Role} from "../enums/role";

export interface AppJwtPayload extends JwtPayload {
    sub: string;
    unique_name: string;
    email: string;
    role: Role;
    jti: string;
}