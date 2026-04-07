using MeetingRoom.Api.Models.Contracts.Requests;
using MeetingRoom.Api.Models.Contracts.Responses;

namespace MeetingRoom.Api.Services;

public interface IReservationService {
    CreateReservationResponse CreateReservation(CreateReservationRequest? request);
    GetReservationsResponse GetReservations();
    DeleteReservationResponse CancelReservation(string reservationId);
}
