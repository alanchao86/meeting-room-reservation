namespace MeetingRoom.Api.Models.Entities;

public sealed class Room {
    public string Id { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = [];
}
