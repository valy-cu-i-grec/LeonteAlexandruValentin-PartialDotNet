using RoomHub.Api.Models;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Services;

public class BookingService(RoomRepository rooms, BookingRepository bookings)
{
    public Booking Create(Guid roomId, string? bookedBy, DateTime startUtc, DateTime endUtc, int attendees)
    {
        var room = rooms.Get(roomId);
        if (room is null)
            throw new BusinessException("RoomNotFound", "The room does not exist.", 404);

        if (string.IsNullOrWhiteSpace(bookedBy))
            throw new BusinessException("InvalidBookedBy", "BookedBy is required.", 400);

        if (endUtc <= startUtc)
            throw new BusinessException("InvalidRange", "The end must be after the start.", 400);

        if (attendees <= 0)
            throw new BusinessException("InvalidAttendees", "Attendees must be positive.", 400);

        if (attendees > room.Capacity)
            throw new BusinessException("AttendeesExceedCapacity", "The room does not have enough seats.", 400);

        foreach (var existing in bookings.ListForRoom(roomId))
        {
            if (existing.StartUtc <= endUtc && startUtc <= existing.EndUtc)
                throw new BusinessException("RoomOccupied", "The room is occupied in this interval.", 409);
        }

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = roomId,
            BookedBy = bookedBy.Trim(),
            StartUtc = startUtc,
            EndUtc = endUtc,
            Attendees = attendees,
            Status = BookingStatus.Active
        };

        bookings.Add(booking);
        return booking;
    }

    public Booking Get(Guid id)
    {
        var booking = bookings.Get(id);
        if (booking is null)
            throw new BusinessException("BookingNotFound", "The booking does not exist.", 404);

        return booking;
    }

    public void Cancel(Guid id)
    {
        var booking = Get(id);
        booking.Status = BookingStatus.Cancelled;
    }
}
