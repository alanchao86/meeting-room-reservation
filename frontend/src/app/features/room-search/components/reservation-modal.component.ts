import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RoomItem } from '../../../core/models/room.model';
import { CreateReservationResponse } from '../../../core/models/reservation.model';
import { ReservationService } from '../../../core/services/reservation.service';
import { FeedbackComponent } from '../../../shared/components/feedback/feedback.component';
import { readApiErrorMessage } from '../../../shared/utils/api-error';
import { DAY_SLOT_COUNT, durationFromSlots, MAX_RESERVATION_SLOTS, slotIndexToTime } from '../../../shared/utils/slot-time';
import { SlotPickerComponent, SlotPickerSlot, SlotRangeSelection } from './slot-picker.component';

@Component({
  selector: 'app-reservation-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, SlotPickerComponent, FeedbackComponent],
  templateUrl: './reservation-modal.component.html',
  styleUrls: ['./reservation-modal.component.css']
})
export class ReservationModalComponent implements OnChanges {
  @Input({ required: true }) room: RoomItem | null = null;
  @Input({ required: true }) date = '';
  @Output() closed = new EventEmitter<void>();
  @Output() created = new EventEmitter<CreateReservationResponse>();

  submitting = false;
  formError = '';
  slotError = '';
  selection: SlotRangeSelection | null = null;

  // Keep a stable array reference for child input; getter-based arrays caused constant resets before.
  slotPickerSlots: SlotPickerSlot[] = [];
  form = this.formBuilder.nonNullable.group({
    event_name: ['', [Validators.required]]
  });

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly reservationService: ReservationService
  ) {}

  get startTimeText(): string {
    if (!this.selection) return '--';
    return slotIndexToTime(this.selection.start_slot_index);
  }

  get endTimeText(): string {
    if (!this.selection) return '--';
    return slotIndexToTime(this.selection.end_slot_index);
  }

  get durationText(): string {
    if (!this.selection) return '--';
    return durationFromSlots(this.selection.end_slot_index - this.selection.start_slot_index);
  }

  get disableSubmit(): boolean {
    if (this.submitting) return true;
    if (!this.room) return true;
    return this.room.available_slots.length === 0;
  }

  // If room or date changes, we reset the form and selection, and rebuild the slot picker
  ngOnChanges(changes: SimpleChanges): void {
    if (!changes['room'] && !changes['date']) return;
    this.form.reset({ event_name: '' });
    this.selection = null;
    this.formError = '';
    this.slotError = '';
    this.submitting = false;
    this.slotPickerSlots = this.buildSlotPickerSlots();
  }

  onSelectionChange(selection: SlotRangeSelection | null): void {
    this.selection = selection;
    this.slotError = '';
    this.formError = '';
  }

  onSelectionInvalid(message: string): void {
    if (!message) {
      this.slotError = '';
      return;
    }

    this.selection = null;
    this.slotError = message;
    this.formError = '';
  }

  onBackdropClick(event: MouseEvent): void {
    if (event.target !== event.currentTarget) return;
    if (this.submitting) return;
    this.onCancel();
  }

  onCancel(): void {
    if (this.submitting) return;
    this.closed.emit();
  }

  // Reservation submission: validate form data, then call API
  onSubmit(): void {
    if (!this.room || this.submitting) return;

    this.form.markAllAsTouched();
    this.formError = '';
    this.slotError = '';

    if (!this.selection) {
      this.slotError = 'Please select a reservable time range.';
      return;
    }

    const durationSlots = this.selection.end_slot_index - this.selection.start_slot_index;
    if (durationSlots < 1 || durationSlots > MAX_RESERVATION_SLOTS) {
      this.formError = 'Please select consecutive slots up to 2 hours.';
      return;
    }

    if (this.form.invalid) return;
    const eventName = this.form.controls.event_name.value.trim();
    if (!eventName) {
      this.form.controls.event_name.setErrors({ required: true });
      return;
    }

    this.submitting = true;
    const payload = {
      room_id: this.room.room_id,
      date: this.date,
      start_slot_index: this.selection.start_slot_index,
      end_slot_index: this.selection.end_slot_index,
      event_name: eventName
    };

    this.reservationService.createReservation(payload).subscribe({
      next: (created) => {
        this.submitting = false;
        this.created.emit(created);
      },
      error: (error) => {
        this.submitting = false;
        this.formError = readApiErrorMessage(error, 'Reservation failed. Please try again.');
      }
    });
  }

  private buildSlotPickerSlots(): SlotPickerSlot[] {
    if (!this.room) return [];

    const availableSet = new Set(this.room.available_slots.map((slot) => slot.slot_index));
    const allSlots: SlotPickerSlot[] = [];

    // Build a complete list of slots for the day and mark them based on availability
    for (let slotIndex = 0; slotIndex < DAY_SLOT_COUNT; slotIndex++) {
      allSlots.push({
        slot_index: slotIndex,
        start_time: slotIndexToTime(slotIndex),
        end_time: slotIndexToTime(slotIndex + 1),
        is_selectable: availableSet.has(slotIndex)
      });
    }

    return allSlots;
  }
}
