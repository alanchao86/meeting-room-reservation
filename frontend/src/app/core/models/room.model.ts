export interface AvailableSlot {
  slot_index: number;
  start_time: string;
  end_time: string;
}

export interface RoomItem {
  room_id: string;
  room_name: string;
  room_number: string;
  capacity: number;
  image_url: string | null;
  available_slots: AvailableSlot[];
  has_reservable_slots: boolean;
}

export interface GetRoomsResponse {
  date: string;
  rooms: RoomItem[];
}
