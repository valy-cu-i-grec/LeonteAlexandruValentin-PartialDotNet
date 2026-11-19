using RoomHub.Api.Common;

namespace RoomHub.Api.Domain.Rooms;

public static class RoomErrors
{
    public static readonly Error InvalidName =
        Error.Validation("InvalidName", "Room name is required.");

    public static readonly Error InvalidCapacity =
        Error.Validation("InvalidCapacity", "Capacity must be positive.");

    public static readonly Error NotFound =
        Error.NotFound("RoomNotFound", "The room does not exist.");
}
