using MeetingRoom.Api.Models.Contracts.Responses;
using MeetingRoom.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MeetingRoom.Api.Controllers;

// Centralized API exception handling
public sealed class ApiExceptionFilter(ILogger<ApiExceptionFilter> logger) : IExceptionFilter {
    private readonly ILogger<ApiExceptionFilter> _logger = logger;

    public void OnException(ExceptionContext context) {
        // Expected business errors: return the code/status defined by service logic.
        if (context.Exception is ApiException apiException) {
            context.Result = new ObjectResult(ErrorResponseFactory.Create(apiException.ErrorCode, apiException.Message)) {
                StatusCode = apiException.StatusCode
            };
            context.ExceptionHandled = true;
            return;
        }

        // Unexpected errors
        _logger.LogError(context.Exception, "Unhandled exception.");
        context.Result = new ObjectResult(ErrorResponseFactory.Create("INTERNAL_ERROR", "Unexpected server error.")) {
            StatusCode = StatusCodes.Status500InternalServerError
        };
        context.ExceptionHandled = true;
    }
}
