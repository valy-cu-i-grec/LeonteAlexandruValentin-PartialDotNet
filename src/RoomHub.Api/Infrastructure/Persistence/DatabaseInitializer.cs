using Microsoft.EntityFrameworkCore;

namespace RoomHub.Api.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    // No migrations are committed to the repo, so the schema is created from the model on first start.
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RoomHubDbContext>();

        if (db.Database.GetMigrations().Any())
            await db.Database.MigrateAsync();
        else
            await db.Database.EnsureCreatedAsync();
    }
}
