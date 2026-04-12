export type UserRole = 'USER' | 'ADMIN';

export interface AuthApiResponse {
  id: number;
  name: string;
  token: string;
}

export interface UserSession {
  id: number;
  name: string;
  email: string;
  role: UserRole;
  token: string;
  expiresAt?: number;
}

interface JwtPayload {
  exp?: number;
  email?: string;
  [key: string]: unknown;
}

const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
const EMAIL_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';
const NAME_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';

function decodeJwtPayload(token: string): JwtPayload {
  const [, payload] = token.split('.');

  if (!payload) {
    throw new Error('JWT payload ausente.');
  }

  const normalizedPayload = payload.replace(/-/g, '+').replace(/_/g, '/');
  const paddedPayload = normalizedPayload.padEnd(normalizedPayload.length + ((4 - normalizedPayload.length % 4) % 4), '=');

  return JSON.parse(atob(paddedPayload)) as JwtPayload;
}

export function mapSessionFromAuth(response: AuthApiResponse): UserSession {
  const payload = decodeJwtPayload(response.token);
  const role = String(payload[ROLE_CLAIM] ?? 'USER').toUpperCase() === 'ADMIN' ? 'ADMIN' : 'USER';
  const email = String(payload[EMAIL_CLAIM] ?? payload.email ?? '');
  const name = String(payload[NAME_CLAIM] ?? response.name ?? 'Chef');
  const expiresAt = typeof payload.exp === 'number' ? payload.exp * 1000 : undefined;

  return {
    id: response.id,
    name,
    email,
    role,
    token: response.token,
    expiresAt
  };
}
