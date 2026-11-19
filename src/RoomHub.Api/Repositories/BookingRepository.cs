using RoomHub.Api.Models;

namespace RoomHub.Api.Repositories;

// Starter: in-memory data. It is lost when the application restarts.
public class BookingRepository
{
    private readonly List<Booking> _bookings = new();
    private readonly object _lock = new();

    public void Add(Booking booking)
    {
        lock (_lock)
        {
            _bookings.Add(booking);
        }
    }

    public Booking? Get(Guid id)
    {
        lock (_lock)
        {
            return _bookings.FirstOrDefault(b => b.Id == id);
        }
    }

    public List<Booking> ListForRoom(Guid roomId)
    {
        lock (_lock)
        {
            return _bookings.Where(b => b.RoomId == roomId).ToList();
        }
    }
}
