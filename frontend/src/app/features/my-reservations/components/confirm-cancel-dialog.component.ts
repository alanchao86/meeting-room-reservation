import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ReservationItem } from '../../../core/models/reservation.model';
import { durationFromMinutes } from '../../../shared/utils/slot-time';

@Component({
  selector: 'app-confirm-cancel-dialog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './confirm-cancel-dialog.component.html',
  styleUrls: ['./confirm-cancel-dialog.component.css']
})
export class ConfirmCancelDialogComponent {
  @Input({ required: true }) reservation: ReservationItem | null = null;
  @Input() busy = false;
  @Output() keep = new EventEmitter<void>();
  @Output() confirm = new EventEmitter<void>();

  get durationText(): string {
    if (!this.reservation) return '--';
    return durationFromMinutes(this.reservation.duration_minutes);
  }

  onKeep(): void {
    if (this.busy) return;
    this.keep.emit();
  }

  onConfirm(): void {
    if (this.busy) return;
    this.confirm.emit();
  }

  // Close dialog when clicking outside the content area
  onBackdropClick(event: MouseEvent): void {
    if (event.target !== event.currentTarget) return;
    this.onKeep();
  }
}
