import { HttpErrorResponse } from '@angular/common/http';
import { ApiErrorResponse } from '../../core/models/api-error.model';

export function readApiErrorMessage(error: unknown, fallbackMessage: string): string {
  if (!(error instanceof HttpErrorResponse)) return fallbackMessage;

  const body = error.error;
  // Backend error format is { error: { code, message } }; 
  // anything else falls back to a safe generic message.
  if (!body || typeof body !== 'object') return fallbackMessage;
  if (!('error' in body)) return fallbackMessage;

  const parsed = body as ApiErrorResponse;
  if (!parsed.error?.message) return fallbackMessage;
  return parsed.error.message;
}
