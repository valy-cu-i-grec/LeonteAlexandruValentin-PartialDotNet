namespace RoomHub.Api.Services;

// Starter: business errors are exceptions, caught by a middleware.
public sealed class BusinessException(string code, string message, int statusCode)
    : Exception(message)
{
    public string Code { get; } = code;
    public int StatusCode { get; } = statusCode;
}
