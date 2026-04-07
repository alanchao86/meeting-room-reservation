namespace MeetingRoom.Api.Models.Entities;

public sealed class User {
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = [];
}
