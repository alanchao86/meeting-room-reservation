namespace MeetingRoom.Api.Services;

// Shared time slot constants so room and reservation logic stay in sync.
public static class SlotTimeHelper {
    public const int DayStartHour = 8;
    public const int SlotMinutes = 30;
    public const int LastSlotIndex = 19;
    public const int EndSlotExclusiveMax = 20;
    public const int MaxReservationSlots = 4; // 2 hours max (4 slots * 30 minutes)

    // Converts a slot boundary index(0-20) to HH:mm text (e.g., 0 -> 08:00, 20 -> 18:00).
    public static string BoundaryToTimeText(int boundaryIndex) {
        var time = TimeOnly.FromTimeSpan(TimeSpan.FromHours(DayStartHour) + TimeSpan.FromMinutes(boundaryIndex * SlotMinutes));
        return time.ToString("HH:mm");
    }
}
