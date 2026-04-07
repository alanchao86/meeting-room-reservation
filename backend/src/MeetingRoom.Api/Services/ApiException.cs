namespace MeetingRoom.Api.Services;

// Service layer throws this, then the controller filter maps it to { error: { code, message } }.
public sealed class ApiException(int statusCode, string errorCode, string message) : Exception(message) {
    public int StatusCode { get; } = statusCode;

    // Error code for frontend
    public string ErrorCode { get; } = errorCode;
}
