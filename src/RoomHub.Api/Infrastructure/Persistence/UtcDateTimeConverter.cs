using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RoomHub.Api.Infrastructure.Persistence;

// SQLite stores text, so EF reads DateTime with Kind Unspecified. We mark the values as UTC.
public sealed class UtcDateTimeConverter()
    : ValueConverter<DateTime, DateTime>(
        value => value,
        value => DateTime.SpecifyKind(value, DateTimeKind.Utc));
