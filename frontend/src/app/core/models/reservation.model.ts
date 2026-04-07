export interface CreateReservationRequest {
  room_id: string;
  date: string;
  start_slot_index: number;
  end_slot_index: number;
  event_name: string;
}

export interface ReservationRoom {
  room_id: string;
  room_name: string;
  room_number: string;
}

export interface ReservationItem {
  reservation_id: string;
  event_name: string;
  date: string;
  start_slot_index: number;
  end_slot_index: number;
  start_time: string;
  end_time: string;
  duration_slots: number;
  duration_minutes: number;
  room: ReservationRoom;
}

export interface GetReservationsResponse {
  reservations: ReservationItem[];
}

export interface CreateReservationResponse {
  reservation_id: string;
  user_id: string;
  room: ReservationRoom;
  date: string;
  start_slot_index: number;
  end_slot_index: number;
  start_time: string;
  end_time: string;
  duration_slots: number;
  duration_minutes: number;
  event_name: string;
  created_at: string;
}

export interface CancelReservationResponse {
  reservation_id: string;
  status: string;
  message: string;
}
