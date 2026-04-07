namespace MeetingRoom.Api.Models.Contracts.Responses;

public sealed class GetRoomsResponse {
    public string Date { get; set; } = string.Empty;
    public List<RoomAvailabilityItemResponse> Rooms { get; set; } = [];
}

public sealed class RoomAvailabilityItemResponse {
    public string RoomId { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string? ImageUrl { get; set; }
    public List<AvailableSlotResponse> AvailableSlots { get; set; } = [];
    public bool HasReservableSlots { get; set; }
}

public sealed class AvailableSlotResponse {
    public int SlotIndex { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
}
