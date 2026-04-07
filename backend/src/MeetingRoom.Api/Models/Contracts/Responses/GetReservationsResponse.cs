namespace MeetingRoom.Api.Models.Contracts.Responses;

public sealed class GetReservationsResponse {
    public List<ReservationItemResponse> Reservations { get; set; } = [];
}

public sealed class ReservationItemResponse {
    public string ReservationId { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public int StartSlotIndex { get; set; }
    public int EndSlotIndex { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int DurationSlots { get; set; }
    public int DurationMinutes { get; set; }
    public RoomSummaryResponse Room { get; set; } = new();
}
