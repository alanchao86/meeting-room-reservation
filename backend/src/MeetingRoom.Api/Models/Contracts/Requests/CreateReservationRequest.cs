namespace MeetingRoom.Api.Models.Contracts.Requests;

public sealed class CreateReservationRequest {
    public string? RoomId { get; set; }
    public string? Date { get; set; }
    public int? StartSlotIndex { get; set; }
    public int? EndSlotIndex { get; set; }
    public string? EventName { get; set; }
}
