using RoomHub.Api.Models;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Services;

public class RoomService(RoomRepository rooms)
{
    public Room Create(string? name, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessException("InvalidName", "Room name is required.", 400);

        if (capacity <= 0)
            throw new BusinessException("InvalidCapacity", "Capacity must be positive.", 400);

        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Capacity = capacity
        };

        rooms.Add(room);
        return room;
    }

    public Room Get(Guid id)
    {
        var room = rooms.Get(id);
        if (room is null)
            throw new BusinessException("RoomNotFound", "The room does not exist.", 404);

        return room;
    }
}
