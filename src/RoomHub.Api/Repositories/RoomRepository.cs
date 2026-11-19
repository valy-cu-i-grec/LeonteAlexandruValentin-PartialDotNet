using RoomHub.Api.Models;

namespace RoomHub.Api.Repositories;

// Starter: in-memory data. It is lost when the application restarts.
public class RoomRepository
{
    private readonly List<Room> _rooms = new();
    private readonly object _lock = new();

    public void Add(Room room)
    {
        lock (_lock)
        {
            _rooms.Add(room);
        }
    }

    public Room? Get(Guid id)
    {
        lock (_lock)
        {
            return _rooms.FirstOrDefault(r => r.Id == id);
        }
    }
}
