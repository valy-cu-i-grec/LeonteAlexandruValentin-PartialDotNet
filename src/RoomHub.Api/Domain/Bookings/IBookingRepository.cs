namespace RoomHub.Api.Domain.Bookings;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<List<Booking>> ListActiveForRoomAsync(Guid roomId, CancellationToken cancellationToken);

    // Active bookings of one person starting inside the UTC day [dayStartUtc, dayStartUtc + 1 day), across all rooms.
    Task<int> CountActiveForBookerOnDayAsync(string bookedBy, DateTime dayStartUtc, CancellationToken cancellationToken);

    void Add(Booking booking);
}
