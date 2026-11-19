using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Common;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public static class CreateBookingEndpoint
{
    public sealed record Request(string? BookedBy, DateTime StartUtc, DateTime EndUtc, int Attendees);

    public sealed record Response(Guid Id, Guid RoomId, BookingStatus Status);

    public static void Map(RouteGroupBuilder group) =>
        group.MapPost("/{roomId:guid}/bookings", Handle).WithName("CreateBooking");

    private static async Task<Results<Created<Response>, ProblemHttpResult>> Handle(
        Guid roomId,
        Request request,
        ICommandHandler<CreateBookingCommand, CreateBookingResult> handler,
        CancellationToken ct)
    {
        var command = new CreateBookingCommand(
            roomId, request.BookedBy, request.StartUtc, request.EndUtc, request.Attendees);

        var result = await handler.Handle(command, ct);
        if (result.IsFailure)
            return result.Error.ToProblem();

        var booking = result.Value;
        return TypedResults.Created(
            $"/api/bookings/{booking.Id}",
            new Response(booking.Id, booking.RoomId, booking.Status));
    }
}
