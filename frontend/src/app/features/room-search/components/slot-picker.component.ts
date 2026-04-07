import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { MAX_RESERVATION_SLOTS } from '../../../shared/utils/slot-time';

export interface SlotPickerSlot {
  slot_index: number;
  start_time: string;
  end_time: string;
  is_selectable: boolean;
}

export interface SlotRangeSelection {
  start_slot_index: number;
  end_slot_index: number;
}

@Component({
  selector: 'app-slot-picker',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './slot-picker.component.html',
  styleUrls: ['./slot-picker.component.css']
})
export class SlotPickerComponent implements OnChanges {
  @Input() slots: SlotPickerSlot[] = [];
  @Output() selectionChange = new EventEmitter<SlotRangeSelection | null>();
  @Output() selectionInvalid = new EventEmitter<string>();

  anchorSlotIndex: number | null = null;
  startSlotIndex: number | null = null;
  endSlotExclusive: number | null = null;

  get sortedSlots(): SlotPickerSlot[] {
    return [...this.slots].sort((a, b) => a.slot_index - b.slot_index);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!changes['slots']) return;
    this.reset();
  }

  reset(): void {
    this.anchorSlotIndex = null;
    this.startSlotIndex = null;
    this.endSlotExclusive = null;
    this.selectionChange.emit(null);
    this.selectionInvalid.emit('');
  }

  onSelect(slotIndex: number): void {
    const target = this.slots.find((slot) => slot.slot_index === slotIndex);
    if (!target || !target.is_selectable) return;

    // First click sets an anchor. Wait for a second click to decide the final range.
    if (this.anchorSlotIndex === null || this.endSlotExclusive !== null) {
      this.pickAnchor(slotIndex);
      this.selectionInvalid.emit('');
      return;
    }

    // User can click backward or forward, we'll normalize into [start, end).
    const start = Math.min(this.anchorSlotIndex, slotIndex);
    const endExclusive = Math.max(this.anchorSlotIndex, slotIndex) + 1;
    const durationSlots = endExclusive - start;
    const available = new Set(this.slots.filter((slot) => slot.is_selectable).map((slot) => slot.slot_index));

    // check the duration limit
    if (durationSlots > MAX_RESERVATION_SLOTS) {
      this.selectionChange.emit(null);
      this.selectionInvalid.emit('Please select consecutive slots up to 2 hours.');
      this.pickAnchor(slotIndex);
      return;
    }

    // Check if all slots in the range are available
    for (let idx = start; idx < endExclusive; idx++) {
      if (available.has(idx)) continue;
      this.selectionChange.emit(null);
      this.selectionInvalid.emit('Please select consecutive slots up to 2 hours.');
      this.pickAnchor(slotIndex);
      return;
    }

    this.startSlotIndex = start;
    this.endSlotExclusive = endExclusive;
    this.selectionInvalid.emit('');
    this.selectionChange.emit({
      start_slot_index: start,
      end_slot_index: endExclusive
    });
  }

  isSelected(slotIndex: number): boolean {
    if (this.startSlotIndex === null || this.endSlotExclusive === null) return false;
    return slotIndex >= this.startSlotIndex && slotIndex < this.endSlotExclusive;
  }

  isAnchor(slotIndex: number): boolean {
    if (this.anchorSlotIndex === null || this.endSlotExclusive !== null) return false;
    return this.anchorSlotIndex === slotIndex;
  }

  private pickAnchor(slotIndex: number): void {
    this.anchorSlotIndex = slotIndex;
    this.startSlotIndex = slotIndex;
    this.endSlotExclusive = null;
    // Clear the old selected range until user picks the second point.
    this.selectionChange.emit(null);
  }
}
