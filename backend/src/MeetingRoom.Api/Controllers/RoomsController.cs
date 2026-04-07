using System.Globalization;
using MeetingRoom.Api.Models.Contracts.Responses;
using MeetingRoom.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoom.Api.Controllers;

[Route("api/rooms")]
public sealed class RoomsController(IRoomService roomAvailabilityService) : ControllerBase {
    private readonly IRoomService _roomAvailabilityService = roomAvailabilityService;

    [HttpGet]
    public IActionResult GetRooms([FromQuery] string? date) {
        // Examine the date query parameter first.
        if (!TryParseDate(date, out var parsedDate)) {
            return BadRequest(ErrorResponseFactory.Create("INVALID_REQUEST", "Invalid or missing date parameter."));
        }

        var response = _roomAvailabilityService.GetRooms(parsedDate);
        return Ok(response);
    }

    private static bool TryParseDate(string? date, out DateOnly parsedDate) =>
        DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);
}
