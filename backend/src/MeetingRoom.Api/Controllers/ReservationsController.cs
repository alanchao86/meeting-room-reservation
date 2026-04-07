using MeetingRoom.Api.Models.Contracts.Requests;
using MeetingRoom.Api.Models.Contracts.Responses;
using MeetingRoom.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoom.Api.Controllers;

[Route("api/reservations")]
public sealed class ReservationsController(IReservationService reservationService) : ControllerBase {
    private readonly IReservationService _reservationService = reservationService;

    [HttpPost]
    public IActionResult CreateReservation([FromBody] CreateReservationRequest? request) {
        // Check the input is well-formed and meets basic validation rules before passing to service
        if (!ModelState.IsValid) return BadRequest(ErrorResponseFactory.Create("INVALID_REQUEST", "Invalid request body."));

        var response = _reservationService.CreateReservation(request);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet]
    public IActionResult GetReservations() {
        var response = _reservationService.GetReservations();
        return Ok(response);
    }

    [HttpDelete("{reservation_id}")]
    public IActionResult CancelReservation([FromRoute(Name = "reservation_id")] string reservationId) {
        // Basic check for invalid input
        if (string.IsNullOrWhiteSpace(reservationId)) return NotFound(ErrorResponseFactory.Create("RESERVATION_NOT_FOUND", "Reservation not found."));

        var response = _reservationService.CancelReservation(reservationId);
        return Ok(response);
    }
}
