using System.Text.Json.Serialization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Features.Bookings;
using RoomHub.Api.Features.Rooms;
using RoomHub.Api.Infrastructure.Persistence;
using RoomHub.Api.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();

// The real clock. Handlers read the time from TimeProvider and pass it to the domain.
builder.Services.AddSingleton(TimeProvider.System);

// A relative database path is resolved against the project folder, so the file does not depend on the working directory.
var connectionString = new SqliteConnectionStringBuilder(
    builder.Configuration.GetConnectionString("RoomHub") ?? "Data Source=roomhub.db");
if (connectionString.DataSource != ":memory:" && !Path.IsPathRooted(connectionString.DataSource))
    connectionString.DataSource = Path.Combine(builder.Environment.ContentRootPath, connectionString.DataSource);

builder.Services.AddDbContext<RoomHubDbContext>(options => options.UseSqlite(connectionString.ToString()));
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<RoomHubDbContext>());
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

builder.Services.AddHandlers(typeof(Program).Assembly);

var app = builder.Build();

await app.InitializeDatabaseAsync();

app.MapGet("/", () => "RoomHub API is running.");
app.MapRooms();
app.MapBookings();

app.Run();
