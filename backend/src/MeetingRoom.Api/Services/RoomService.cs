using System.Globalization;
using MeetingRoom.Api.Data;
using MeetingRoom.Api.Models.Contracts.Responses;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoom.Api.Services;

public sealed class RoomService(AppDbContext dbContext) : IRoomService {
    private readonly AppDbContext _dbContext = dbContext;

    // Builds room card data for the selected date
    public GetRoomsResponse GetRooms(DateOnly date) {
        // only allows today+ datess
        if (date < DateOnly.FromDateTime(DateTime.Now)) {
            throw new ApiException(StatusCodes.Status422UnprocessableEntity, "INVALID_DATE", "Date cannot be earlier than today.");
        }

        // Sort by room number
        var rooms = _dbContext.Rooms
            .AsNoTracking()
            .OrderBy(x => x.RoomNumber)
            .ToList();

        // Get Grouped reservations by room
        var reservationsByRoom = _dbContext.Reservations
            .AsNoTracking()
            .Where(x => x.ReservationDate == date)
            .ToList()
            .GroupBy(x => x.RoomId)
            .ToDictionary(x => x.Key, x => x.ToList());

        var roomResponses = new List<RoomAvailabilityItemResponse>(rooms.Count);

        foreach (var room in rooms) {
            var occupiedSlots = new bool[SlotTimeHelper.EndSlotExclusiveMax];

            // Mark occupied slots based on reservations for this room
            if (reservationsByRoom.TryGetValue(room.Id, out var roomReservations)) {
                // end_slot_index is exclusive, we mark [start, end).
                foreach (var reservation in roomReservations) {
                    for (var slotIndex = reservation.StartSlotIndex; slotIndex < reservation.EndSlotIndex; slotIndex++) {
                        occupiedSlots[slotIndex] = true;
                    }
                }
            }

            var availableSlots = new List<AvailableSlotResponse>();
            for (var slotIndex = 0; slotIndex <= SlotTimeHelper.LastSlotIndex; slotIndex++) {
                if (occupiedSlots[slotIndex]) continue;

                // Frontend uses this list directly for available slot buttons
                availableSlots.Add(new AvailableSlotResponse {
                    SlotIndex = slotIndex,
                    StartTime = SlotTimeHelper.BoundaryToTimeText(slotIndex),
                    EndTime = SlotTimeHelper.BoundaryToTimeText(slotIndex + 1)
                });
            }

            roomResponses.Add(new RoomAvailabilityItemResponse {
                RoomId = room.Id,
                RoomName = room.RoomName,
                RoomNumber = room.RoomNumber,
                Capacity = room.Capacity,
                ImageUrl = room.ImageUrl,
                AvailableSlots = availableSlots,
                HasReservableSlots = availableSlots.Count > 0
            });
        }

        return new GetRoomsResponse {
            Date = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            Rooms = roomResponses
        };
    }
}
