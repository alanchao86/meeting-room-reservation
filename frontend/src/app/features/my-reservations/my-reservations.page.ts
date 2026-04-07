import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ReservationItem } from '../../core/models/reservation.model';
import { ReservationService } from '../../core/services/reservation.service';
import { FeedbackComponent } from '../../shared/components/feedback/feedback.component';
import { readApiErrorMessage } from '../../shared/utils/api-error';
import { durationFromMinutes } from '../../shared/utils/slot-time';
import { ConfirmCancelDialogComponent } from './components/confirm-cancel-dialog.component';

type NoticeType = 'success' | 'error';

@Component({
  selector: 'app-my-reservations-page',
  standalone: true,
  imports: [CommonModule, FeedbackComponent, ConfirmCancelDialogComponent],
  templateUrl: './my-reservations.page.html',
  styleUrls: ['./my-reservations.page.css']
})
export class MyReservationsPageComponent implements OnInit {
  reservations: ReservationItem[] = [];
  pendingCancel: ReservationItem | null = null;
  loading = false;
  loadError = '';
  canceling = false;
  noticeMessage = '';
  noticeType: NoticeType = 'success';

  constructor(private readonly reservationService: ReservationService) {}

  ngOnInit(): void {
    this.loadReservations();
  }

  onRetry(): void {
    this.loadReservations();
  }

  onOpenCancelDialog(item: ReservationItem): void {
    this.pendingCancel = item;
  }

  onKeepReservation(): void {
    if (this.canceling) return;
    this.pendingCancel = null;
  }

  // Cancel the reservation
  onConfirmCancel(): void {
    if (!this.pendingCancel || this.canceling) return;

    this.canceling = true;
    const target = this.pendingCancel;
    // API call to cancel the reservation
    this.reservationService.cancelReservation(target.reservation_id).subscribe({
      next: () => {
        this.canceling = false;
        this.pendingCancel = null;
        // Remove the canceled reservation from the list
        this.reservations = this.reservations.filter((item) => item.reservation_id !== target.reservation_id);
        this.noticeType = 'success';
        this.noticeMessage = 'Reservation canceled successfully.';
      },
      error: (error) => {
        this.canceling = false;
        this.pendingCancel = null;
        this.noticeType = 'error';
        this.noticeMessage = readApiErrorMessage(error, 'Cancellation failed. Please try again.');
      }
    });
  }

  getDuration(item: ReservationItem): string {
    return durationFromMinutes(item.duration_minutes);
  }

  trackReservation(index: number, item: ReservationItem): string {
    return `${index}-${item.reservation_id}`;
  }

  // Load user's reservations
  private loadReservations(): void {
    this.loading = true;
    this.loadError = '';
    this.noticeMessage = '';

    this.reservationService.getReservations().subscribe({
      next: (response) => {
        this.loading = false;
        this.reservations = response.reservations ?? [];
      },
      error: (error) => {
        this.loading = false;
        this.reservations = [];
        this.loadError = readApiErrorMessage(error, 'Unable to load reservations. Retry.');
      }
    });
  }
}
