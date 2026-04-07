import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RoomItem } from '../../core/models/room.model';
import { RoomService } from '../../core/services/room.service';
import { FeedbackComponent } from '../../shared/components/feedback/feedback.component';
import { readApiErrorMessage } from '../../shared/utils/api-error';
import { getTodayDateInputValue } from '../../shared/utils/slot-time';
import { ReservationModalComponent } from './components/reservation-modal.component';
import { RoomCardComponent } from './components/room-card.component';

@Component({
  selector: 'app-room-search-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    FeedbackComponent,
    RoomCardComponent,
    ReservationModalComponent
  ],
  templateUrl: './room-search.page.html',
  styleUrls: ['./room-search.page.css']
})
export class RoomSearchPageComponent implements OnInit {
  today = getTodayDateInputValue();
  selectedDate = this.today;
  rooms: RoomItem[] = [];
  activeRoom: RoomItem | null = null;
  loading = false;
  loadError = '';
  successMessage = '';

  constructor(private readonly roomService: RoomService) {}

  ngOnInit(): void {
    this.loadRooms();
  }

  onDateChange(): void {
    this.successMessage = '';
    this.loadRooms();
  }

  onRetry(): void {
    this.loadRooms();
  }

  onOpenReserve(room: RoomItem): void {
    this.activeRoom = room;
  }

  onCloseModal(): void {
    this.activeRoom = null;
  }

  onReservationCreated(): void {
    this.activeRoom = null;
    this.successMessage = 'Reservation created successfully.';
    this.loadRooms();
  }

  trackRoom(index: number, room: RoomItem): string {
    return `${index}-${room.room_id}`;
  }

  private loadRooms(): void {
    this.loading = true;
    this.loadError = '';

    this.roomService.getRooms(this.selectedDate).subscribe({
      next: (response) => {
        this.loading = false;
        this.rooms = response.rooms ?? [];
      },
      error: (error) => {
        this.loading = false;
        this.rooms = [];
        this.loadError = readApiErrorMessage(error, 'Unable to load rooms. Retry.');
      }
    });
  }
}
