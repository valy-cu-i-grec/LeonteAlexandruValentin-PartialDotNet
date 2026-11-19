using RoomHub.Api.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Domain.Bookings;

public enum BookingStatus
{
    Active,
    Cancelled
}

public sealed class Booking
{
    public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(4);
    public static readonly TimeSpan MaxDurationLargeRoom = TimeSpan.FromHours(2);
    public const int LargeRoomCapacityThreshold = 10;
    public static readonly TimeSpan MinCancelNotice = TimeSpan.FromMinutes(60);
    public const int MaxActivePerDay = 2;

    private Booking()
    {
    }

    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string BookedBy { get; private set; } = string.Empty;
    public DateRange Range { get; private set; } = null!;
    public int Attendees { get; private set; }
    public BookingStatus Status { get; private set; }

    public static Result<Booking> Create(
        Room room,
        string? bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(bookedBy))
            return BookingErrors.InvalidBookedBy;

        var rangeResult = DateRange.Create(startUtc, endUtc);
        if (rangeResult.IsFailure)
            return rangeResult.Error;

        if (attendees <= 0)
            return BookingErrors.InvalidAttendees;

        if (attendees > room.Capacity)
            return BookingErrors.AttendeesExceedCapacity;

        var range = rangeResult.Value;

        if (range.StartUtc < nowUtc)
            return BookingErrors.StartInPast;

        // R10: rooms with capacity above 10 allow at most 2 hours, the others keep the 4-hour limit (R6).
        if (room.Capacity > LargeRoomCapacityThreshold)
        {
            if (range.Duration > MaxDurationLargeRoom)
                return BookingErrors.TooLongForLargeRoom;
        }
        else if (range.Duration > MaxDuration)
        {
            return BookingErrors.TooLong;
        }

        return new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            BookedBy = bookedBy.Trim(),
            Range = range,
            Attendees = attendees,
            Status = BookingStatus.Active
        };
    }

    // Two bookings conflict only if both are active, in the same room, with overlapping intervals.
    public bool ConflictsWith(Booking other) =>
        Status == BookingStatus.Active
        && other.Status == BookingStatus.Active
        && RoomId == other.RoomId
        && Range.Overlaps(other.Range);

    public Result<Unit> Cancel(DateTime nowUtc)
    {
        if (Status == BookingStatus.Cancelled)
            return BookingErrors.AlreadyCancelled;

        // R11: cancelling is allowed only if the booking starts at least 60 minutes from now.
        if (Range.StartUtc - nowUtc < MinCancelNotice)
            return BookingErrors.CancellationTooLate;

        Status = BookingStatus.Cancelled;
        return Unit.Value;
    }
}
