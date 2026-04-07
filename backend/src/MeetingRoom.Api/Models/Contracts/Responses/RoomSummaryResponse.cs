namespace MeetingRoom.Api.Models.Contracts.Responses;

public sealed class RoomSummaryResponse {
    public string RoomId { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
}
