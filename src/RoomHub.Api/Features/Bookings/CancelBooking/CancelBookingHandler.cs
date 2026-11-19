using RoomHub.Api.Common;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public sealed record CancelBookingCommand(Guid BookingId) : ICommand<Unit>;

public sealed class CancelBookingHandler(IBookingRepository bookings, IUnitOfWork unitOfWork)
    : ICommandHandler<CancelBookingCommand, Unit>
{
    public async Task<Result<Unit>> Handle(
        CancelBookingCommand command, CancellationToken cancellationToken)
    {
        var booking = await bookings.GetByIdAsync(command.BookingId, cancellationToken);
        if (booking is null)
            return BookingErrors.NotFound;

        var result = booking.Cancel();
        if (result.IsFailure)
            return result.Error;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
