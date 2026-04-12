export interface ApiErrorResponse {
  statusCode: number;
  message: string;
  details?: string | null;
  traceId?: string | null;
  errors?: Record<string, string[]>;
}

