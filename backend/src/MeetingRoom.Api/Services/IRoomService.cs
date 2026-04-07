using MeetingRoom.Api.Models.Contracts.Responses;

namespace MeetingRoom.Api.Services;

public interface IRoomService {
    GetRoomsResponse GetRooms(DateOnly date);
}
