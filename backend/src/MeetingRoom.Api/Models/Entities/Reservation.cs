namespace MeetingRoom.Api.Models.Entities;

public sealed class Reservation {
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string RoomId { get; set; } = string.Empty;
    public DateOnly ReservationDate { get; set; }
    public int StartSlotIndex { get; set; }
    public int EndSlotIndex { get; set; }
    public string EventName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User? User { get; set; }
    public Room? Room { get; set; }
}
