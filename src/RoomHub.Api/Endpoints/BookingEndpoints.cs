using RoomHub.Api.Services;

namespace RoomHub.Api.Endpoints;

public record BookingResponse(
    Guid Id,
    Guid RoomId,
    string BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees,
    string Status);

public static class BookingEndpoints
{
    public static void MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/bookings").WithTags("Bookings");

        group.MapGet("/{id:guid}", (Guid id, BookingService service) =>
        {
            var b = service.Get(id);
            return Results.Ok(new BookingResponse(
                b.Id, b.RoomId, b.BookedBy, b.StartUtc, b.EndUtc, b.Attendees, b.Status.ToString()));
        });

        group.MapPost("/{id:guid}/cancel", (Guid id, BookingService service) =>
        {
            service.Cancel(id);
            return Results.NoContent();
        });
    }
}
