using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Common;
using RoomHub.Api.Common.Cqrs;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public static class CreateRoomEndpoint
{
    public sealed record Request(string? Name, int Capacity);

    public sealed record Response(Guid Id, string Name, int Capacity);

    public static void Map(RouteGroupBuilder group) =>
        group.MapPost("/", Handle).WithName("CreateRoom");

    private static async Task<Results<Created<Response>, ProblemHttpResult>> Handle(
        Request request,
        ICommandHandler<CreateRoomCommand, CreateRoomResult> handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(new CreateRoomCommand(request.Name, request.Capacity), ct);
        if (result.IsFailure)
            return result.Error.ToProblem();

        var room = result.Value;
        return TypedResults.Created(
            $"/api/rooms/{room.Id}",
            new Response(room.Id, room.Name, room.Capacity));
    }
}
