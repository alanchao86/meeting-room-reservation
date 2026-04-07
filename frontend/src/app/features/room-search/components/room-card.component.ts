import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RoomItem } from '../../../core/models/room.model';

@Component({
  selector: 'app-room-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './room-card.component.html',
  styleUrls: ['./room-card.component.css']
})
export class RoomCardComponent {
  @Input({ required: true }) room!: RoomItem;
  @Output() reserve = new EventEmitter<RoomItem>();

  onReserve(): void {
    this.reserve.emit(this.room);
  }

  trackSlot(index: number, _slot: unknown): number {
    return index;
  }
}
