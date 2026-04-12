import { Injectable, signal } from '@angular/core';

export type ToastTone = 'success' | 'error' | 'info';

export interface ToastMessage {
  id: number;
  tone: ToastTone;
  title: string;
  message: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private nextId = 0;
  private readonly state = signal<ToastMessage[]>([]);
  readonly messages = this.state.asReadonly();

  show(title: string, message: string, tone: ToastTone = 'info', durationMs = 4200): void {
    this.nextId += 1;

    const toast: ToastMessage = {
      id: this.nextId,
      title,
      message,
      tone
    };

    this.state.update((messages) => [...messages, toast]);
    window.setTimeout(() => this.dismiss(toast.id), durationMs);
  }

  success(title: string, message: string): void {
    this.show(title, message, 'success');
  }

  error(title: string, message: string): void {
    this.show(title, message, 'error', 5400);
  }

  dismiss(id: number): void {
    this.state.update((messages) => messages.filter((message) => message.id !== id));
  }
}

