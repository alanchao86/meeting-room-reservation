namespace MeetingRoom.Api.Models.Contracts.Responses;

public sealed class ErrorResponse {
    public required ErrorBody Error { get; init; }
}

public sealed class ErrorBody {
    public required string Code { get; init; }
    public required string Message { get; init; }
}

public static class ErrorResponseFactory {
    public static ErrorResponse Create(string code, string message) {
        return new ErrorResponse {
            Error = new ErrorBody {
                Code = code,
                Message = message
            }
        };
    }
}
