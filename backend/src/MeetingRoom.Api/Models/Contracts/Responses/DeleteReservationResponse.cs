namespace MeetingRoom.Api.Models.Contracts.Responses;

public sealed class DeleteReservationResponse {
    public string ReservationId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
