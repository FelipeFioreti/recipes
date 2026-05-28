export interface LoginPayload {
    email: string;
    password: string;
}

export interface AuthResponse {
    id: number;
    name: string;
    token: string;
}