using System.Globalization;
using MeetingRoom.Api.Data;
using MeetingRoom.Api.Models.Contracts.Requests;
using MeetingRoom.Api.Models.Contracts.Responses;
using MeetingRoom.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoom.Api.Services;

public sealed class ReservationService(AppDbContext dbContext) : IReservationService {
    private const string DefaultUserId = "default-user"; // Assume single default user for prototype
    private const string InvalidSlotRangeMessage = "Please select consecutive slots up to 2 hours.";
    private readonly AppDbContext _dbContext = dbContext;

    // Check all booking rules and create a reservation
    public CreateReservationResponse CreateReservation(CreateReservationRequest? request) {
        // Check necessary fields and date format
        if (request is null ||
            string.IsNullOrWhiteSpace(request.RoomId) ||
            string.IsNullOrWhiteSpace(request.Date) ||
            request.StartSlotIndex is null ||
            request.EndSlotIndex is null ||
            !DateOnly.TryParseExact(request.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var reservationDate) ||
            string.IsNullOrWhiteSpace(request.EventName)) {
            throw new ApiException(StatusCodes.Status400BadRequest, "INVALID_REQUEST", "Invalid request body.");
        }

        var startSlotIndex = request.StartSlotIndex.Value;
        var endSlotIndex = request.EndSlotIndex.Value;
        var durationSlots = endSlotIndex - startSlotIndex;

        // Check time slot rules
        if (reservationDate < DateOnly.FromDateTime(DateTime.Now) ||
            startSlotIndex < 0 ||
            startSlotIndex > SlotTimeHelper.LastSlotIndex ||
            endSlotIndex < 1 ||
            endSlotIndex > SlotTimeHelper.EndSlotExclusiveMax ||
            startSlotIndex >= endSlotIndex ||
            durationSlots < 1 ||
            durationSlots > SlotTimeHelper.MaxReservationSlots) {
            throw new ApiException(StatusCodes.Status422UnprocessableEntity, "INVALID_SLOT_RANGE", InvalidSlotRangeMessage);
        }

        // check room exists
        var room = _dbContext.Rooms.AsNoTracking().FirstOrDefault(x => x.Id == request.RoomId);
        if (room is null) {
            throw new ApiException(StatusCodes.Status404NotFound, "ROOM_NOT_FOUND", "Room not found.");
        }

        // check for slot conflicts with existing reservations
        var hasConflict = _dbContext.Reservations.Any(x =>
            x.RoomId == request.RoomId &&
            x.ReservationDate == reservationDate &&
            startSlotIndex < x.EndSlotIndex &&
            endSlotIndex > x.StartSlotIndex);
        if (hasConflict) {
            throw new ApiException(
                StatusCodes.Status409Conflict,
                "SLOT_CONFLICT",
                "Selected slots are no longer available. Please reselect.");
        }

        // check user exists
        if (!_dbContext.Users.Any(x => x.Id == DefaultUserId)) {
            throw new ApiException(StatusCodes.Status500InternalServerError, "INTERNAL_ERROR", "Unexpected server error.");
        }

        var utcNow = DateTime.UtcNow;
        var reservation = new Reservation {
            Id = $"resv-{Guid.NewGuid():N}",
            UserId = DefaultUserId,
            RoomId = room.Id,
            ReservationDate = reservationDate,
            StartSlotIndex = startSlotIndex,
            EndSlotIndex = endSlotIndex,
            EventName = request.EventName.Trim(),
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };

        _dbContext.Reservations.Add(reservation);
        _dbContext.SaveChanges();

        // Return ready-to-render values so frontend does not need extra conversion
        return new CreateReservationResponse {
            ReservationId = reservation.Id,
            UserId = reservation.UserId,
            Room = new RoomSummaryResponse {
                RoomId = room.Id,
                RoomName = room.RoomName,
                RoomNumber = room.RoomNumber
            },
            Date = reservationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            StartSlotIndex = startSlotIndex,
            EndSlotIndex = endSlotIndex,
            StartTime = SlotTimeHelper.BoundaryToTimeText(startSlotIndex),
            EndTime = SlotTimeHelper.BoundaryToTimeText(endSlotIndex),
            DurationSlots = durationSlots,
            DurationMinutes = durationSlots * SlotTimeHelper.SlotMinutes,
            EventName = reservation.EventName,
            CreatedAt = reservation.CreatedAt
        };
    }

    // Get "My Reservations" list in date/time order.
    public GetReservationsResponse GetReservations() {
        // reservations should besorted by date, start slot.
        var reservations = _dbContext.Reservations
            .AsNoTracking()
            .Where(x => x.UserId == DefaultUserId)
            .Include(x => x.Room)
            .OrderBy(x => x.ReservationDate)
            .ThenBy(x => x.StartSlotIndex)
            .ToList();

        // Map entities to response items
        var items = reservations.Select(reservation => {
            var durationSlots = reservation.EndSlotIndex - reservation.StartSlotIndex;
            var room = reservation.Room;

            return new ReservationItemResponse {
                ReservationId = reservation.Id,
                EventName = reservation.EventName,
                Date = reservation.ReservationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                StartSlotIndex = reservation.StartSlotIndex,
                EndSlotIndex = reservation.EndSlotIndex,
                StartTime = SlotTimeHelper.BoundaryToTimeText(reservation.StartSlotIndex),
                EndTime = SlotTimeHelper.BoundaryToTimeText(reservation.EndSlotIndex),
                DurationSlots = durationSlots,
                DurationMinutes = durationSlots * SlotTimeHelper.SlotMinutes,
                Room = new RoomSummaryResponse {
                    RoomId = room?.Id ?? string.Empty,
                    RoomName = room?.RoomName ?? string.Empty,
                    RoomNumber = room?.RoomNumber ?? string.Empty
                }
            };
        }).ToList();

        return new GetReservationsResponse {
            Reservations = items
        };
    }

    // Delete a reservation
    public DeleteReservationResponse CancelReservation(string reservationId) {
        var reservation = _dbContext.Reservations.FirstOrDefault(x => x.Id == reservationId && x.UserId == DefaultUserId);
        if (reservation is null) {
          throw new ApiException(StatusCodes.Status404NotFound, "RESERVATION_NOT_FOUND", "Reservation not found.");  
        } 

        _dbContext.Reservations.Remove(reservation);
        _dbContext.SaveChanges();

        return new DeleteReservationResponse {
            ReservationId = reservationId,
            Status = "canceled",
            Message = "Reservation canceled successfully."
        };
    }
}
