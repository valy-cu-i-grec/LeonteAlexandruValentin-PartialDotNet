using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Features.Bookings.GetBookingById;

public static class GetBookingByIdEndpoint
{
    public sealed record Response(
        Guid Id,
        Guid RoomId,
        string BookedBy,
        DateTime StartUtc,
        DateTime EndUtc,
        int Attendees,
        BookingStatus Status);

    public static void Map(RouteGroupBuilder group) =>
        group.MapGet("/{id:guid}", Handle).WithName("GetBookingById");

    private static async Task<Results<Ok<Response>, ProblemHttpResult>> Handle(
        Guid id,
        IBookingRepository bookings,
        CancellationToken ct)
    {
        var booking = await bookings.GetByIdAsync(id, ct);
        if (booking is null)
            return BookingErrors.NotFound.ToProblem();

        return TypedResults.Ok(new Response(
            booking.Id,
            booking.RoomId,
            booking.BookedBy,
            booking.Range.StartUtc,
            booking.Range.EndUtc,
            booking.Attendees,
            booking.Status));
    }
}
