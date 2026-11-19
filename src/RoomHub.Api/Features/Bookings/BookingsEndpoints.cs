using RoomHub.Api.Features.Bookings.CancelBooking;
using RoomHub.Api.Features.Bookings.CreateBooking;
using RoomHub.Api.Features.Bookings.GetBookingById;

namespace RoomHub.Api.Features.Bookings;

public static class BookingsEndpoints
{
    public static IEndpointRouteBuilder MapBookings(this IEndpointRouteBuilder app)
    {
        // The contract creates a booking under the room: POST /api/rooms/{roomId}/bookings
        var roomGroup = app.MapGroup("/api/rooms").WithTags("Bookings");
        CreateBookingEndpoint.Map(roomGroup);

        var group = app.MapGroup("/api/bookings").WithTags("Bookings");
        GetBookingByIdEndpoint.Map(group);
        CancelBookingEndpoint.Map(group);

        return app;
    }
}
