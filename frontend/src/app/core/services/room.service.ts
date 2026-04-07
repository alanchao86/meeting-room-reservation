import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GetRoomsResponse } from '../models/room.model';

@Injectable({ providedIn: 'root' })
export class RoomService {
  constructor(private readonly http: HttpClient) {}

  getRooms(date: string): Observable<GetRoomsResponse> {
    return this.http.get<GetRoomsResponse>('/api/rooms', { params: { date } });
  }
}
