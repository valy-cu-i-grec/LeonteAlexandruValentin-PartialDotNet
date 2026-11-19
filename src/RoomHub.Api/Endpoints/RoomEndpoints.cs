using RoomHub.Api.Services;

namespace RoomHub.Api.Endpoints;

public record CreateRoomRequest(string? Name, int Capacity);

public record RoomResponse(Guid Id, string Name, int Capacity);

public record CreateBookingRequest(string? BookedBy, DateTime StartUtc, DateTime EndUtc, int Attendees);

public record BookingCreatedResponse(Guid Id, Guid RoomId, string Status);

public static class RoomEndpoints
{
    public static void MapRoomEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/rooms").WithTags("Rooms");

        group.MapPost("/", (CreateRoomRequest request, RoomService service) =>
        {
            var room = service.Create(request.Name, request.Capacity);
            return Results.Created(
                $"/api/rooms/{room.Id}",
                new RoomResponse(room.Id, room.Name, room.Capacity));
        });

        group.MapGet("/{id:guid}", (Guid id, RoomService service) =>
        {
            var room = service.Get(id);
            return Results.Ok(new RoomResponse(room.Id, room.Name, room.Capacity));
        });

        group.MapPost("/{roomId:guid}/bookings", (Guid roomId, CreateBookingRequest request, BookingService service) =>
        {
            var booking = service.Create(
                roomId, request.BookedBy, request.StartUtc, request.EndUtc, request.Attendees);

            return Results.Created(
                $"/api/bookings/{booking.Id}",
                new BookingCreatedResponse(booking.Id, booking.RoomId, booking.Status.ToString()));
        });
    }
}
