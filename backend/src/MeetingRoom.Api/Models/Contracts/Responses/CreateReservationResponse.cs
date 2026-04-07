namespace MeetingRoom.Api.Models.Contracts.Responses;

public sealed class CreateReservationResponse {
    public string ReservationId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public RoomSummaryResponse Room { get; set; } = new();
    public string Date { get; set; } = string.Empty;
    public int StartSlotIndex { get; set; }
    public int EndSlotIndex { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int DurationSlots { get; set; }
    public int DurationMinutes { get; set; }
    public string EventName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
