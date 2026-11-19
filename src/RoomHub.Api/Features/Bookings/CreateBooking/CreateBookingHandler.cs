using RoomHub.Api.Common;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingCommand(
    Guid RoomId,
    string? BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees) : ICommand<CreateBookingResult>;

public sealed record CreateBookingResult(Guid Id, Guid RoomId, BookingStatus Status);

public sealed class CreateBookingHandler(
    IRoomRepository rooms,
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider time) : ICommandHandler<CreateBookingCommand, CreateBookingResult>
{
    public async Task<Result<CreateBookingResult>> Handle(
        CreateBookingCommand command, CancellationToken cancellationToken)
    {
        // The room is checked first: a missing room is 404 before any validation.
        var room = await rooms.GetByIdAsync(command.RoomId, cancellationToken);
        if (room is null)
            return RoomErrors.NotFound;

        var created = Booking.Create(
            room,
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            time.GetUtcNow().UtcDateTime);
        if (created.IsFailure)
            return created.Error;

        var booking = created.Value;

        // R7: only active bookings of the same room block the interval.
        var activeInRoom = await bookings.ListActiveForRoomAsync(room.Id, cancellationToken);
        if (activeInRoom.Any(existing => booking.ConflictsWith(existing)))
            return BookingErrors.RoomOccupied;

        // R9: at most 2 active bookings per person per UTC day (of startUtc), across all rooms.
        var activeThatDay = await bookings.CountActiveForBookerOnDayAsync(
            booking.BookedBy, booking.Range.StartUtc.Date, cancellationToken);
        if (activeThatDay >= Booking.MaxActivePerDay)
            return BookingErrors.DailyLimitExceeded;

        bookings.Add(booking);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateBookingResult(booking.Id, booking.RoomId, booking.Status);
    }
}
