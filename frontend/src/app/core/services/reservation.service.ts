import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CancelReservationResponse,
  CreateReservationRequest,
  CreateReservationResponse,
  GetReservationsResponse
} from '../models/reservation.model';

@Injectable({ providedIn: 'root' })
export class ReservationService {
  constructor(private readonly http: HttpClient) {}

  getReservations(): Observable<GetReservationsResponse> {
    return this.http.get<GetReservationsResponse>('/api/reservations');
  }

  createReservation(payload: CreateReservationRequest): Observable<CreateReservationResponse> {
    return this.http.post<CreateReservationResponse>('/api/reservations', payload);
  }

  cancelReservation(reservationId: string): Observable<CancelReservationResponse> {
    return this.http.delete<CancelReservationResponse>(`/api/reservations/${reservationId}`);
  }
}
