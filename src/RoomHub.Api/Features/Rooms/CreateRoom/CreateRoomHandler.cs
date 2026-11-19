using RoomHub.Api.Common;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public sealed record CreateRoomCommand(string? Name, int Capacity) : ICommand<CreateRoomResult>;

public sealed record CreateRoomResult(Guid Id, string Name, int Capacity);

public sealed class CreateRoomHandler(IRoomRepository rooms, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateRoomCommand, CreateRoomResult>
{
    public async Task<Result<CreateRoomResult>> Handle(
        CreateRoomCommand command, CancellationToken cancellationToken)
    {
        var result = Room.Create(command.Name, command.Capacity);
        if (result.IsFailure)
            return result.Error;

        var room = result.Value;
        rooms.Add(room);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateRoomResult(room.Id, room.Name, room.Capacity);
    }
}
